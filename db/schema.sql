BEGIN;

CREATE EXTENSION IF NOT EXISTS moddatetime;

SET TIME ZONE 'UTC';

CREATE TABLE IF NOT EXISTS app_user
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    email varchar(255) UNIQUE NOT NULL CHECK (email ~* '^[A-Za-z0-9._+%-]+@[A-Za-z0-9.-]+[.][A-Za-z]+$'),
    password varchar(128) NOT NULL,
    first_name varchar(50) NOT NULL,
    last_name varchar(50) NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT user_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS address
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    city varchar(50) NOT NULL,
    street varchar(150) NOT NULL,
    building varchar(10) NOT NULL,
    apartment varchar(10),
    region_id integer,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS region
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    region_name varchar(50),
    PRIMARY KEY (id),
    UNIQUE (region_name)
);

CREATE TABLE IF NOT EXISTS user_address
(
    user_id integer,
    address_id integer,
    is_default boolean,
    PRIMARY KEY (user_id, address_id)
);

CREATE TABLE IF NOT EXISTS record
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    title varchar(150) NOT NULL,
    description text,
    image_url varchar(150),
    release_date date NOT NULL,
    duration_seconds integer NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS artist
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    name varchar(50) NOT NULL,
    description text,
    image_url varchar(150),
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS format
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    format_name varchar(50) NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (id),
    UNIQUE (format_name)
);

CREATE TABLE IF NOT EXISTS product
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    record_id integer NOT NULL,
    format_id integer NOT NULL,
    description text,
    quantity integer NOT NULL,
    price money NOT NULL,
    inactive boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS discount
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    product_id integer NOT NULL,
    start_date date NOT NULL,
    end_date date NOT NULL,
    discount_percent integer NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (id),
    CONSTRAINT discount_percent CHECK (discount_percent >= 0 AND discount_percent <= 100)
);

CREATE TABLE IF NOT EXISTS artist_record
(
    artist_id integer NOT NULL,
    record_id integer NOT NULL,
    PRIMARY KEY (artist_id, record_id)
);

CREATE TABLE IF NOT EXISTS genre
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    name varchar NOT NULL UNIQUE,
    description text,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS genre_record
(
    genre_id integer NOT NULL,
    record_id integer NOT NULL,
    PRIMARY KEY (genre_id, record_id)
);

CREATE TABLE IF NOT EXISTS genre_artist
(
    genre_id integer NOT NULL,
    artist_id integer NOT NULL,
    PRIMARY KEY (genre_id, artist_id)
);

CREATE TABLE IF NOT EXISTS track
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    title varchar NOT NULL,
    description text,
    duration_seconds integer NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS track_record
(
    track_id integer NOT NULL,
    record_id integer NOT NULL,
    track_order integer NOT NULL,
    PRIMARY KEY (track_id, record_id)
);

CREATE TABLE IF NOT EXISTS track_artist
(
    track_id integer NOT NULL,
    artist_id integer NOT NULL,
    PRIMARY KEY (track_id, artist_id)
);

CREATE TABLE IF NOT EXISTS shopping_cart
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    user_id integer NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS shopping_cart_product
(
    shopping_cart_id integer NOT NULL,
    product_id integer NOT NULL,
    quantity integer,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (shopping_cart_id, product_id)
);

CREATE TABLE IF NOT EXISTS shop_order
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    user_id integer NOT NULL,
    total money NOT NULL,
    city varchar(50) NOT NULL,
    street varchar(150) NOT NULL,
    building varchar(10) NOT NULL,
    apartment varchar(10),
    status_id integer NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS order_line
(
    shop_order_id integer NOT NULL,
    product_id integer NOT NULL,
    quantity integer NOT NULL,
    price money NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (shop_order_id, product_id)
);

CREATE TABLE IF NOT EXISTS review
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    user_id integer NOT NULL,
    product_id integer NOT NULL,
    rating integer NOT NULL,
    description text,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS order_status
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    name varchar(15) NOT NULL,
    description text,
    PRIMARY KEY (id)
);

ALTER TABLE IF EXISTS address
    ADD CONSTRAINT address_region_id_fkey FOREIGN KEY (region_id)
    REFERENCES region (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS user_address
    ADD FOREIGN KEY (user_id)
    REFERENCES app_user (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS user_address
    ADD FOREIGN KEY (address_id)
    REFERENCES address (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS product
    ADD FOREIGN KEY (record_id)
    REFERENCES record (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS product
    ADD FOREIGN KEY (format_id)
    REFERENCES format (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS discount
    ADD FOREIGN KEY (product_id)
    REFERENCES product (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS artist_record
    ADD FOREIGN KEY (artist_id)
    REFERENCES artist (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS artist_record
    ADD FOREIGN KEY (record_id)
    REFERENCES record (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS genre_record
    ADD FOREIGN KEY (genre_id)
    REFERENCES genre (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS genre_record
    ADD FOREIGN KEY (record_id)
    REFERENCES record (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS genre_artist
    ADD FOREIGN KEY (genre_id)
    REFERENCES genre (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS genre_artist
    ADD FOREIGN KEY (artist_id)
    REFERENCES artist (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS track_record
    ADD FOREIGN KEY (track_id)
    REFERENCES track (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS track_record
    ADD FOREIGN KEY (record_id)
    REFERENCES record (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS track_artist
    ADD FOREIGN KEY (track_id)
    REFERENCES track (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS track_artist
    ADD FOREIGN KEY (artist_id)
    REFERENCES artist (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS shopping_cart
    ADD FOREIGN KEY (user_id)
    REFERENCES app_user (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS shopping_cart_product
    ADD FOREIGN KEY (shopping_cart_id)
    REFERENCES shopping_cart (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS shopping_cart_product
    ADD FOREIGN KEY (product_id)
    REFERENCES product (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS shop_order
    ADD FOREIGN KEY (user_id)
    REFERENCES app_user (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE SET NULL
    NOT VALID;


ALTER TABLE IF EXISTS shop_order
    ADD FOREIGN KEY (status_id)
    REFERENCES order_status (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS order_line
    ADD FOREIGN KEY (shop_order_id)
    REFERENCES shop_order (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS order_line
    ADD FOREIGN KEY (product_id)
    REFERENCES product (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE SET NULL
    NOT VALID;


ALTER TABLE IF EXISTS review
    ADD FOREIGN KEY (user_id)
    REFERENCES app_user (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE SET NULL
    NOT VALID;

ALTER TABLE IF EXISTS review
    ADD FOREIGN KEY (product_id)
    REFERENCES product (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;

CREATE TRIGGER user_updated_at
    BEFORE UPDATE ON app_user
    FOR EACH ROW
    EXECUTE PROCEDURE moddatetime('updated_at');

CREATE TRIGGER address_updated_at
    BEFORE UPDATE ON address
    FOR EACH ROW
    EXECUTE PROCEDURE moddatetime('updated_at');

CREATE TRIGGER region_updated_at
    BEFORE UPDATE ON region
    FOR EACH ROW
    EXECUTE PROCEDURE moddatetime('updated_at');

CREATE TRIGGER record_updated_at
    BEFORE UPDATE ON record
    FOR EACH ROW
    EXECUTE PROCEDURE moddatetime('updated_at');

CREATE TRIGGER artist_updated_at
    BEFORE UPDATE ON artist
    FOR EACH ROW
    EXECUTE PROCEDURE moddatetime('updated_at');

CREATE TRIGGER format_updated_at
    BEFORE UPDATE ON format
    FOR EACH ROW
    EXECUTE PROCEDURE moddatetime('updated_at');

CREATE TRIGGER product_updated_at
    BEFORE UPDATE ON product
    FOR EACH ROW
    EXECUTE PROCEDURE moddatetime('updated_at');

CREATE TRIGGER discount_updated_at
    BEFORE UPDATE ON discount
    FOR EACH ROW
    EXECUTE PROCEDURE moddatetime('updated_at');

CREATE TRIGGER genre_updated_at
    BEFORE UPDATE ON genre
    FOR EACH ROW
    EXECUTE PROCEDURE moddatetime('updated_at');

CREATE TRIGGER track_updated_at
    BEFORE UPDATE ON track
    FOR EACH ROW
    EXECUTE PROCEDURE moddatetime('updated_at');

CREATE TRIGGER shopping_cart_product_updated_at
    BEFORE UPDATE ON shopping_cart_product
    FOR EACH ROW
    EXECUTE PROCEDURE moddatetime('updated_at');

CREATE TRIGGER review_updated_at
    BEFORE UPDATE ON review
    FOR EACH ROW
    EXECUTE PROCEDURE moddatetime('updated_at');

END;