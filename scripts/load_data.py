import re
import xml.etree.ElementTree as ET
from sqlobject.converters import sqlrepr
from rich.progress import Progress, BarColumn, TimeRemainingColumn, TextColumn
from model import *
import psycopg2 as pg

progress = Progress(
    TextColumn("[bold blue]{task.description}", justify="right"),
    BarColumn(bar_width=None),
    "[progress.percentage]{task.percentage:>3.1f}%",
    TimeRemainingColumn(),
)


def connect():
    pg_password = "Jd35B^1-W\"a_/}0,"

    return pg.connect(dbname="RecordStoreDB",
                      user="postgres",
                      password=pg_password,
                      host="localhost",
                      port="5432")


def remove_brackets(text):
    pattern = r'\\?\[(l=|a=)(.+?)\\]'
    return re.sub(pattern, r'\2', text)


def fix_date(date):
    if date is None:
        return None

    date = date.split('-')

    if len(date) == 1:
        return date[0] + '-01-01'

    elif len(date) == 2:
        return date[0] + '-' + date[1] + '-01'

    if date[1] == '00':
        return date[0] + '-01-01'

    if date[2] == '00':
        return date[0] + '-' + date[1] + '-01'

    return date[0] + '-' + date[1] + '-' + date[2]


def parse_artist_xml(file_path):
    tree = ET.parse(file_path)
    root = tree.getroot()
    artists = [parse_artist(artist) for artist in root.iter('artist')]
    return artists


def parse_artist(artist):
    name = sqlrepr(artist.find('name').text, 'postgres')
    profile = artist.find('profile').text
    profile = sqlrepr(remove_brackets(profile),
                      'postgres') if profile else None

    return Artist(name, profile, int(artist.find('id').text))


def process_artists(artists, cursor):
    task = progress.add_task("[red]Processing Artists...", total=len(artists))
    with progress:
        for artist in artists:
            cursor.execute(
                """
                    INSERT INTO artist (name, description)
                    VALUES (%s, %s)
                    RETURNING id;
                """,
                (
                    artist.name,
                    artist.description
                ))
            db_id = cursor.fetchone()[0]

            artist.database_id = db_id
            progress.advance(task)

    progress.remove_task(task)

# -------------------
# release xml parsing


def parse_release_xml(file_path, artists):
    """
    ### Returns
    ---
    #### list[tuple[Record, list[ArtistRecord]]]
        A list of tuples containing the record and a list of ArtistRecord objects.

        ArtistRecord objects are used to link the record to the artists
        after they have been inserted into the database.
    """
    artists_ids = [artist.dataset_id for artist in artists]

    records = []
    count = 0

    context = ET.iterparse(file_path, events=("start", "end"))
    context = iter(context)
    event, root = next(context)  # Get the root element

    for event, element in context:
        if event == "end" and element.tag == "release":
            dataset_artists = element.find('artists')
            if dataset_artists is not None:
                artist_ids = [int(a.find('id').text) for a in dataset_artists]
                if any(artist_id in artists_ids for artist_id in artist_ids):
                    record, artist_records, genres = parse_record(
                        element, artists)
                    records.append((record, artist_records, genres))
                    count += 1

                    if count % 1000 == 0:
                        progress.console.print(
                            f"[bold green]{count} records parsed...")
                    if count >= 10000:                                   # for testing purposes
                        break
            root.clear()  # Free up memory

    progress.console.print("[bold green]Records parsed successfully.")
    return records


def parse_record(release, artists) -> tuple[Record, list[ArtistRecord]]:
    """
    This function has to be called after the artists have been inserted into the database.
    ### Parameters
    ---
    #### `release: xml.etree.ElementTree.Element`
        The release element from the XML file.
    #### artists: list[Artist]
        A list of Artist objects.
    ### Returns
    ---
    #### tuple[Record, list[ArtistRecord], list[Genre]]
        A tuple containing the record, a list of ArtistRecord objects, and a list of Genre objects.
    """
    title = sqlrepr(release.find('title').text, 'postgres')

    release_date = release.find('released')

    release_date = release_date.text if release_date is not None else None

    release_date = fix_date(release_date)

    dataset_artists = release.find('artists')

    artistRecords = []

    # need to check for duplocates for some reason

    dataset_ids = []
    if dataset_artists is not None:
        for a in dataset_artists:
            dataset_id = int(a.find('id').text)

            if dataset_id in dataset_ids:
                continue

            dataset_ids.append(dataset_id)

            matching_artist_ids = [
                artist.database_id for artist in artists if artist.dataset_id == dataset_id]

            if matching_artist_ids:
                artistRecords.append(ArtistRecord(
                    matching_artist_ids[0], None))

    genres = genre if (genre := release.find(
        'genres')) is not None else None
    styles = style if (style := release.find(
        'styles')) is not None else None

    record_genres = []
    if genres:
        for genre in genres:
            record_genres.append(Genre(genre.text))

    if styles:
        for style in styles:
            record_genres.append(Genre(style.text))

    return (Record(title, release_date=release_date, dataset_id=int(release.get('id'))), artistRecords, record_genres)


def insert_record(cursor, record):
    """
    Insert a record into the database and return its ID.
    """
    cursor.execute(
        """
        INSERT INTO record (title, release_date)
        VALUES (%s, %s)
        RETURNING id;
        """,
        (record.title, record.release_date)
    )
    return cursor.fetchone()[0]


def insert_artist_record_mapping(cursor, record_id, artist_record):
    """
    Insert a mapping between a record and an artist.
    """
    artist_record.record_id = record_id
    cursor.execute(
        """
        INSERT INTO artist_record (record_id, artist_id)
        VALUES (%s, %s);
        """,
        (artist_record.record_id, artist_record.artist_id)
    )


def get_or_create_genre(cursor, genre_name):
    """
    Get the ID of a genre, or create a new genre if it doesn't exist.
    """
    cursor.execute(
        """
        SELECT id FROM genre
        WHERE name = %s;
        """,
        (genre_name,)
    )

    if cursor.rowcount == 0:
        cursor.execute(
            """
            INSERT INTO genre (name)
            VALUES (%s)
            ON CONFLICT (name) DO NOTHING
            RETURNING id;
            """,
            (genre_name,)
        )
        genre_id = cursor.fetchone()[0]
    else:
        genre_id = cursor.fetchone()[0]

    return genre_id


def insert_genre_record_mapping(cursor, record_id, genre_id):
    """
    Insert a mapping between a record and a genre.
    """
    cursor.execute(
        """
        INSERT INTO genre_record (record_id, genre_id)
        VALUES (%s, %s)
        ON CONFLICT DO NOTHING;
        """,
        (record_id, genre_id)
    )


def insert_genre_artist_mapping(cursor, artist_id, genre_id):
    """
    Insert a mapping between an artist and a genre.
    """
    cursor.execute(
        """
        INSERT INTO genre_artist (artist_id, genre_id)
        VALUES (%s, %s)
        ON CONFLICT DO NOTHING;
        """,
        (artist_id, genre_id)
    )


def process_records(records, cursor):
    """
    Process the records and their associated artists and genres.
    """
    task = progress.add_task("[red]Processing Records...", total=len(records))

    with progress:
        for record_tuple in records:
            record, artist_records, genres = record_tuple

            # Insert the record
            record.database_id = insert_record(cursor, record)

            # Insert artist-record mappings
            for artist_record in artist_records:
                insert_artist_record_mapping(
                    cursor, record.database_id, artist_record)

            # Process genres
            for genre in genres:
                genre.database_id = get_or_create_genre(cursor, genre.name)
                insert_genre_record_mapping(
                    cursor, record.database_id, genre.database_id)

                for artist_record in artist_records:
                    insert_genre_artist_mapping(
                        cursor, artist_record.artist_id, genre.database_id)

            progress.advance(task)


def main():
    with connect() as conn:
        cursor = conn.cursor()

        with progress.console.status("[bold green]Processing Artists XML file..."):
            artists = parse_artist_xml(
                r'D:\DBMusicDataset\artists_correct.xml')
        process_artists(artists, cursor)

        with progress.console.status("[bold green]Processing Releases XML file..."):
            records = parse_release_xml(
                r'D:\DBMusicDataset\discogs_20240301_releases.xml', artists)
        process_records(records, cursor)

        conn.commit()
        # conn.rollback()  # for testing purposes


if __name__ == '__main__':
    main()
