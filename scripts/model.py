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


# It don't have no dataset_id yo.
class Track:
    def __init__(self, title, duration_seconds, database_id=None):
        self.title = title
        self.duration = duration_seconds
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


class Product:
    def __init__(self, record_id, formats, description, dataset_id, price, quantity, inactive=False, database_id=None):
        self.record_id = record_id
        self.formats = formats
        self.description = description
        self.dataset_id = dataset_id
        self.price = price
        self.quantity = quantity
        self.inactive = inactive
        self.database_id = database_id

    def set_database_id(self, database_id):
        self.database_id = database_id


class TrackProduct:
    def __init__(self, track_id, product_id, track_order, database_id=None):
        self.track_id = track_id
        self.product_id = product_id
        self.track_order = track_order
        self.database_id = database_id

    def set_database_id(self, database_id):
        self.database_id = database_id


class RecordProduct:
    def __init__(self, record_id, product_id, database_id=None):
        self.record_id = record_id
        self.product_id = product_id
        self.database_id = database_id

    def set_database_id(self, database_id):
        self.database_id = database_id
