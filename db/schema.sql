-- This script was generated by the ERD tool in pgAdmin 4.
-- Please log an issue at https://github.com/pgadmin-org/pgadmin4/issues/new/choose if you find any bugs, including reproduction steps.
BEGIN;


CREATE TABLE IF NOT EXISTS public."user"
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    email character varying(255) COLLATE pg_catalog."default",
    password character varying(128) COLLATE pg_catalog."default",
    first_name character varying(50) COLLATE pg_catalog."default",
    last_name character varying(50) COLLATE pg_catalog."default",
    created_at timestamp with time zone DEFAULT now(),
    CONSTRAINT users_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.address
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    city character varying(50) NOT NULL,
    street character varying(150) NOT NULL,
    building character varying(10) NOT NULL,
    apartment character varying(10),
    region_id integer,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.region
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    region_name character varying(50),
    PRIMARY KEY (id),
    UNIQUE (region_name)
);

CREATE TABLE IF NOT EXISTS public.user_address
(
    user_id integer,
    address_id integer,
    is_default boolean,
    PRIMARY KEY (user_id, address_id)
);

CREATE TABLE IF NOT EXISTS public.record
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    title character varying(150) NOT NULL,
    description text,
    image_url character varying(150),
    release_date date NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.artist
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    name character varying(50) NOT NULL,
    description text,
    image_url character varying(150),
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.format
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    format_name character varying(50) NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.product
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    record_id integer NOT NULL,
    format_id integer NOT NULL,
    description text,
    quantity integer NOT NULL,
    price money NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.discount
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    product_id integer NOT NULL,
    start_date date NOT NULL,
    end_date date NOT NULL,
    discount integer,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.artist_record
(
    artist_id integer NOT NULL,
    record_id integer NOT NULL,
    PRIMARY KEY (artist_id, record_id)
);

ALTER TABLE IF EXISTS public.address
    ADD CONSTRAINT address_region_id_fkey FOREIGN KEY (region_id)
    REFERENCES public.region (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.user_address
    ADD FOREIGN KEY (user_id)
    REFERENCES public."user" (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.user_address
    ADD FOREIGN KEY (address_id)
    REFERENCES public.address (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.product
    ADD FOREIGN KEY (record_id)
    REFERENCES public.record (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.product
    ADD FOREIGN KEY (format_id)
    REFERENCES public.format (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.discount
    ADD FOREIGN KEY (product_id)
    REFERENCES public.product (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.artist_record
    ADD FOREIGN KEY (artist_id)
    REFERENCES public.artist (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.artist_record
    ADD FOREIGN KEY (record_id)
    REFERENCES public.record (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;

END;