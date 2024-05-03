import lxml.etree as ET
from rich.progress import Progress, BarColumn, TimeRemainingColumn, TextColumn
from model import *
import psycopg2 as pg
from utils import *
import datetime

progress = Progress(
    TextColumn("[bold blue]{task.description}", justify="right"),
    BarColumn(bar_width=None),
    "[progress.percentage]{task.percentage:>3.1f}%",
    TimeRemainingColumn(),
)
# CONSTANTS
DB_NAME = "RecordStoreDB"
DB_USER = "postgres"
DB_PASSWORD = "Jd35B^1-Wa_/}0,"
DB_HOST = "localhost"
DB_PORT = "5432"


image_urls = {
    "Vinyl": "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b6/12in-Vinyl-LP-Record-Angle.jpg/800px-12in-Vinyl-LP-Record-Angle.jpg",
    "CD": "https://images-platform.99static.com//IQ3GhyogoO6ALruR2vbh7HKVd9o=/94x184:1644x1734/fit-in/500x500/99designs-contests-attachments/84/84134/attachment_84134420",
    "CDr": "https://images-platform.99static.com//IQ3GhyogoO6ALruR2vbh7HKVd9o=/94x184:1644x1734/fit-in/500x500/99designs-contests-attachments/84/84134/attachment_84134420",
    "DVD": "https://images-platform.99static.com//IQ3GhyogoO6ALruR2vbh7HKVd9o=/94x184:1644x1734/fit-in/500x500/99designs-contests-attachments/84/84134/attachment_84134420",
    "Cassette": "https://m.media-amazon.com/images/I/619ncpA5k1L.jpg",
    "Box Set": "https://jazz.centerstagestore.com/cdn/shop/products/LP-Boxeset.png?v=1612994585",
    "fall back": "https://media.istockphoto.com/id/1175435360/vector/music-note-icon-vector-illustration.jpg?s=612x612&w=0&k=20&c=R7s6RR849L57bv_c7jMIFRW4H87-FjLB8sqZ08mN0OU="
}

RECORD_COUNT = 5000
PRODUCTS_COUNT = 6000


def connect():
    return pg.connect(dbname=DB_NAME,
                      user=DB_USER,
                      password=DB_PASSWORD,
                      host=DB_HOST,
                      port=DB_PORT)


def parse_artist_xml(file_path):
    tree = ET.parse(file_path)
    root = tree.getroot()
    artists = [parse_artist(artist) for artist in root.iter('artist')]
    return artists


def parse_artist(artist):
    name = artist.find('name').text
    profile = artist.find('profile').text
    profile = remove_brackets(profile) if profile else None

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


def parse_master_xml(file_path, artists):
    """
    ### Returns
    ---
    #### list[tuple[Record, list[ArtistRecord]]]
        A list of tuples containing the record and a list of ArtistRecord objects.

        ArtistRecord objects are used to link the record to the artists
        after they have been inserted into the database.
    """
    db_artist_ids = [artist.dataset_id for artist in artists]

    records = []
    count = 0

    context = ET.iterparse(file_path, events=("start", "end"))
    context = iter(context)
    event, root = next(context)  # Get the root element

    for event, element in context:
        if event == "end" and element.tag == "master":
            dataset_artists = element.find('artists')
            if dataset_artists is not None:
                artist_ids = [int(artist.find('id').text)
                              for artist in dataset_artists]
                if any(artist_id in db_artist_ids for artist_id in artist_ids):
                    record, artist_records, genres = parse_record(
                        element, artists)
                    records.append((record, artist_records, genres))
                    count += 1

                    if count % 1000 == 0:
                        progress.console.print(
                            f"[bold green]{count} records parsed...")
                    if count >= RECORD_COUNT:                                   # for testing purposes
                        break
            root.clear()  # Free up memory

    progress.console.print("[bold green]Records parsed successfully.")
    return records


def parse_record(xml_master, db_artists) -> tuple[Record, list[ArtistRecord]]:
    """
    This function has to be called after the artists have been inserted into the database.
    ### Parameters
    ---
    #### `release: xml.etree.ElementTree.Element`
        The release element from the XML file.
    #### `artists: list[Artist]`
        A list of Artist objects.
    ### Returns
    ---
    #### `tuple[Record, list[ArtistRecord], list[Genre]]`
        A tuple containing the record, a list of ArtistRecord objects, and a list of Genre objects.
    """

    title = xml_master.find('title').text

    release_date = xml_master.find('year')

    release_date = release_date.text if release_date is not None else None

    release_date = fix_date(release_date)

    dataset_artists = xml_master.find('artists')

    artistRecords = []

    # need to check for duplicates for some reason

    dataset_ids = []
    if dataset_artists is not None:
        for a in dataset_artists:
            dataset_id = int(a.find('id').text)

            if dataset_id in dataset_ids:
                continue

            dataset_ids.append(dataset_id)

            matching_artist_ids = [
                artist.database_id for artist in db_artists if artist.dataset_id == dataset_id]

            if matching_artist_ids:
                artistRecords.append(ArtistRecord(
                    matching_artist_ids[0], None))

    genres = genre if (genre := xml_master.find(
        'genres')) is not None else None
    styles = style if (style := xml_master.find(
        'styles')) is not None else None

    record_genres = []
    if genres is not None:
        for genre in genres:
            record_genres.append(Genre(genre.text))

    if styles is not None:
        for style in styles:
            record_genres.append(Genre(style.text))

    return (Record(title, release_date=release_date, dataset_id=int(xml_master.get('id'))), artistRecords, record_genres)


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
    task = progress.add_task("[red]Processing Releases...", total=len(records))

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

            progress.advance(task)
    progress.remove_task(task)


def parse_release_xml(file_path, record_artists_tuples):

    products = []

    context = ET.iterparse(file_path, events=("start", "end"))
    context = iter(context)
    event, root = next(context)  # Get the root element

    count = 0
    total_count = 0
    record_dict = {record.dataset_id: record for record,
                   _, _ in record_artists_tuples}
    for event, element in context:
        if event == "end" and element.tag == "release":
            total_count += 1
            release_master_id = element.find('master_id')

            if release_master_id is not None:
                release_master_id = int(release_master_id.text)

                master_record = record_dict.get(release_master_id)

                if master_record is not None:
                    product_tuple = parse_product(element, master_record)
                    products.append(product_tuple)
                    count += 1

                    if count % 100 == 0:
                        progress.console.print(
                            f"[bold green]{count} products parsed...")
                        progress.console.print(
                            f"[bold green]{total_count} products parsed in total...")

                if count >= PRODUCTS_COUNT:                                   # for testing purposes
                    break
        root.clear()  # Free up memory

    return products


def parse_product(xml_release, record):
    record_id = record.database_id

    formats = xml_release.find('formats')
    if formats is None:
        format_names = []
        format_descriptions = []
    else:
        format_names = [format.get('name') for format in formats]
        format_descriptions = []
        for format in formats:
            descriptions = format.find('descriptions')
            if descriptions is not None:
                description_texts = [
                    description.text for description in descriptions.findall('description')]
                format_descriptions.append(description_texts)
            else:
                format_descriptions.append([])

    labels = xml_release.find('labels')

    if labels is not None:
        label = labels[0].get('name')
    else:
        label = None

    id = xml_release.get('id', '')

    description = ""

    tracks_xml = xml_release.find('tracklist')
    if tracks_xml is not None:
        for track_xml in tracks_xml:
            title, duration, track_order = parse_track(track_xml)
            description += f"{track_order}. {title} - {datetime.timedelta(seconds=duration)}\n"

    if format_descriptions:
        formatted_descriptions = [", ".join(desc)
                                  for desc in format_descriptions if desc]
        if formatted_descriptions:
            description += "\nFormat notes: " + \
                ", ".join(formatted_descriptions)

    notes = xml_release.find('notes')
    if notes is not None and notes.text:
        description += "\n\nNotes: " + notes.text

    dataset_id = int(id) if id else -1

    price = random_price()
    quantity = random_quantity()

    product = Product(
        record_id, format_names if format_names else [], label, description, dataset_id, price, quantity)

    return product


def parse_track(track_xml):
    title = track_xml.find('title').text if track_xml.find(
        'title') is not None else ""
    duration_xml = track_xml.find('duration')

    if title == "Channel Check":
        pass

    duration = get_duration_seconds(
        duration_xml.text if duration_xml is not None else None)

    track_order = track_xml.find('position').text if track_xml.find(
        'position') is not None else ""

    return title, duration, track_order


def get_duration_seconds(duration):
    if duration is None:
        return 0

    parts = duration.split(':')

    if len(parts) == 1:
        parts = duration.split('.')

    if len(parts) == 1:
        return int(parts[0])

    if len(parts) == 2:
        return int(parts[0]) * 60 + int(parts[1])

    if len(parts) == 3:
        return int(parts[0]) * 3600 + int(parts[1]) * 60 + int(parts[2])


def process_products(products, cursor):
    task = progress.add_task(
        "[red]Processing Products...", total=len(products)
    )

    with progress:
        for product in products:
            format_id = get_or_create_format(cursor, product.formats)
            label_id = get_or_create_label(cursor, product.label)

            product.database_id = insert_product(
                cursor,
                product.record_id,
                format_id,
                label_id,
                product.description,
                product.quantity,
                product.price,
                product.formats[0],
            )

            progress.advance(task)
    progress.remove_task(task)


def get_or_create_format(cursor, format_names):
    format_id = None
    for format_name in format_names:
        cursor.execute(
            """
            SELECT id FROM format
            WHERE format_name = %s;
            """,
            (format_name,),
        )

        if cursor.rowcount == 0:
            cursor.execute(
                """
                INSERT INTO format (format_name)
                VALUES (%s)
                RETURNING id;
                """,
                (format_name,),
            )

        format_id = cursor.fetchone()[0]

    return format_id


def get_or_create_label(cursor, label_name):
    if label_name is None:
        return None

    cursor.execute(
        """
        SELECT id FROM label
        WHERE name = %s;
        """,
        (label_name,),
    )

    if cursor.rowcount == 0:
        cursor.execute(
            """
            INSERT INTO label (name)
            VALUES (%s)
            RETURNING id;
            """,
            (label_name,),
        )

    label_id = cursor.fetchone()[0]

    return label_id


def insert_product(
    cursor, record_id, format_id, label_id, description, quantity, price, format_name
):
    cursor.execute(
        """
        INSERT INTO product (record_id, format_id, description, price, image_url, label_id)
        VALUES (%s, %s, %s, %s, %s, %s)
        RETURNING id;
        """,
        (
            record_id,
            format_id,
            description,
            price,
            image_urls.get(format_name, image_urls["fall back"]),
            label_id,
        ),
    )
    product_id = cursor.fetchone()[0]

    cursor.execute(
        """
        INSERT INTO inventory (product_id, quantity, location, restock_level)
        VALUES (%s, %s, %s, %s);
        """,
        (product_id, quantity, "Row 1, Shelf 1", 100),
    )

    return product_id


def run():
    with connect() as conn:
        cursor = conn.cursor()

        cursor.execute("select * from truncate_schema('public')")

        with progress.console.status("[bold green]Processing Artists XML file..."):
            artists = parse_artist_xml(
                r'./artists_correct.xml')
        process_artists(artists, cursor)

        with progress.console.status("[bold green]Processing Masters XML file..."):
            record_artists_tuples = parse_master_xml(
                r'./discogs_20240301_masters.xml', artists)
        process_records(record_artists_tuples, cursor)

        with progress.console.status("[bold green]Processing Releases XML file..."):
            products = parse_release_xml(
                r'./discogs_20240301_releases.xml', record_artists_tuples)

        process_products(products, cursor)

        conn.commit()
        # conn.rollback()  # for testing purposes
