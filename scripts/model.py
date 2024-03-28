class Artist:
    def __init__(self, name, profile, dataset_id, database_id=None):
        self.name = name
        self.description = profile
        self.dataset_id = dataset_id
        self.database_id = database_id

    def set_database_id(self, database_id):
        self.database_id = database_id

    def __str__(self):
        return f"Name: {self.name}\nProfile: {self.description}\n"

    def __repr__(self):
        return f"Name: {self.name}\nProfile: {self.description}\n"


class Record:
    def __init__(self, title, dataset_id, database_id=None, description=None, release_date=None):
        self.title = title
        self.description = description
        self.release_date = release_date
        self.dataset_id = dataset_id
        self.database_id = database_id

    def set_database_id(self, database_id):
        self.database_id = database_id

    def __str__(self):
        return f"Title: {self.title}\nDescription: {self.description}\nRelease Date: {self.release_date}\n"

    def __repr__(self):
        return f"Title: {self.title}\nDescription: {self.description}\nRelease Date: {self.release_date}\n"


class Track:
    def __init__(self, title, duration_seconds, dataset_id, database_id=None):
        self.title = title
        self.duration = duration_seconds
        self.dataset_id = dataset_id
        self.database_id = database_id

    def set_database_id(self, database_id):
        self.database_id = database_id

    def __str__(self):
        return f"Title: {self.title}\nDuration: {self.duration}\n"

    def __repr__(self):
        return f"Title: {self.title}\nDuration: {self.duration}\n"


class TrackArtist:
    def __init__(self, track_id, artist_id, artist_database_id=None):
        self.track_id = track_id
        self.artist_id = artist_id
        self.artist_database_id = artist_database_id

    def set_artist_database_id(self, artist_database_id):
        self.artist_database_id = artist_database_id

    def __str__(self):
        return f"Track ID: {self.track_id}\nArtist ID: {self.artist_id}\n"

    def __repr__(self):
        return f"Track ID: {self.track_id}\nArtist ID: {self.artist_id}\n"


class TrackRecord:
    def __init__(self, track_id, record_id, track_order, record_database_id=None):
        self.track_id = track_id
        self.record_id = record_id
        self.track_order = track_order
        self.record_database_id = record_database_id

    def set_record_database_id(self, record_database_id):
        self.record_database_id = record_database_id

    def __str__(self):
        return f"Track ID: {self.track_id}\nRecord ID: {self.record_id}\nTrack Order: {self.track_order}\n"

    def __repr__(self):
        return f"Track ID: {self.track_id}\nRecord ID: {self.record_id}\nTrack Order: {self.track_order}\n"


class ArtistRecord:
    """
    At the point of creation, the database id of artist and record should not be known.
    """

    def __init__(self, artist_id, record_id):
        self.artist_id = artist_id
        self.record_id = record_id

    def __str__(self):
        return f"Artist ID: {self.artist_id}\nRecord ID: {self.record_id}\n"

    def __repr__(self):
        return f"Artist ID: {self.artist_id}\nRecord ID: {self.record_id}\n"


class Genre:
    """
    Note that discogs stores genres as tags. Therefore, there is no dataset_id for genre.
    Also, the 'style' tag will not be differentiated from the 'genre' tag.
    """

    def __init__(self, name, database_id=None):
        self.name = name


class TrackGenre:
    def __init__(self, track_id, genre_id):
        self.track_id = track_id
        self.genre_id = genre_id

    def set_database_id(self, database_id):
        self.database_id = database_id

    def __str__(self):
        return f"Track ID: {self.track_id}\nGenre ID: {self.genre_id}\n"

    def __repr__(self):
        return f"Track ID: {self.track_id}\nGenre ID: {self.genre_id}\n"


class GenreRecord:
    def __init__(self, record_id, genre_id):
        self.record_id = record_id
        self.genre_id = genre_id

    def __str__(self):
        return f"Record ID: {self.record_id}\nGenre ID: {self.genre_id}\n"

    def __repr__(self):
        return f"Record ID: {self.record_id}\nGenre ID: {self.genre_id}\n"


class GenreArtist:
    def __init__(self, artist_id, genre_id):
        self.artist_id = artist_id
        self.genre_id = genre_id

    def __str__(self):
        return f"Artist ID: {self.artist_id}\nGenre ID: {self.genre_id}\n"

    def __repr__(self):
        return f"Artist ID: {self.artist_id}\nGenre ID: {self.genre_id}\n"
