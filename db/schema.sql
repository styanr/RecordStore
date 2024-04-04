--
-- PostgreSQL database dump
--

-- Dumped from database version 16.2 (Debian 16.2-1.pgdg120+2)
-- Dumped by pg_dump version 16.2

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 2 (class 3079 OID 33081)
-- Name: moddatetime; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS moddatetime WITH SCHEMA public;


--
-- TOC entry 3558 (class 0 OID 0)
-- Dependencies: 2
-- Name: EXTENSION moddatetime; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION moddatetime IS 'functions for tracking last modification time';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 219 (class 1259 OID 33095)
-- Name: address; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.address (
    id integer NOT NULL,
    city character varying(50) NOT NULL,
    street character varying(150) NOT NULL,
    building character varying(10) NOT NULL,
    apartment character varying(10),
    region_id integer,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.address OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 33094)
-- Name: address_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.address ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.address_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 217 (class 1259 OID 33084)
-- Name: app_user; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.app_user (
    id integer NOT NULL,
    email character varying(255) NOT NULL,
    password character varying(128) NOT NULL,
    first_name character varying(50) NOT NULL,
    last_name character varying(50) NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    CONSTRAINT app_user_email_check CHECK (((email)::text ~* '^[A-Za-z0-9._+%-]+@[A-Za-z0-9.-]+[.][A-Za-z]+$'::text))
);


ALTER TABLE public.app_user OWNER TO postgres;

--
-- TOC entry 216 (class 1259 OID 33083)
-- Name: app_user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.app_user ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.app_user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 226 (class 1259 OID 33126)
-- Name: artist; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.artist (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    description text,
    image_url character varying(150),
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.artist OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 33125)
-- Name: artist_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.artist ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.artist_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 233 (class 1259 OID 33165)
-- Name: artist_record; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.artist_record (
    artist_id integer NOT NULL,
    record_id integer NOT NULL
);


ALTER TABLE public.artist_record OWNER TO postgres;

--
-- TOC entry 232 (class 1259 OID 33157)
-- Name: discount; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.discount (
    id integer NOT NULL,
    product_id integer NOT NULL,
    start_date date NOT NULL,
    end_date date NOT NULL,
    discount_percent integer NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    CONSTRAINT discount_percent CHECK (((discount_percent >= 0) AND (discount_percent <= 100)))
);


ALTER TABLE public.discount OWNER TO postgres;

--
-- TOC entry 231 (class 1259 OID 33156)
-- Name: discount_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.discount ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.discount_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 228 (class 1259 OID 33136)
-- Name: format; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.format (
    id integer NOT NULL,
    format_name character varying(50) NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.format OWNER TO postgres;

--
-- TOC entry 227 (class 1259 OID 33135)
-- Name: format_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.format ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.format_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 235 (class 1259 OID 33171)
-- Name: genre; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.genre (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.genre OWNER TO postgres;

--
-- TOC entry 237 (class 1259 OID 33187)
-- Name: genre_artist; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.genre_artist (
    genre_id integer NOT NULL,
    artist_id integer NOT NULL
);


ALTER TABLE public.genre_artist OWNER TO postgres;

--
-- TOC entry 234 (class 1259 OID 33170)
-- Name: genre_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.genre ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.genre_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 236 (class 1259 OID 33182)
-- Name: genre_record; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.genre_record (
    genre_id integer NOT NULL,
    record_id integer NOT NULL
);


ALTER TABLE public.genre_record OWNER TO postgres;

--
-- TOC entry 245 (class 1259 OID 33232)
-- Name: order_line; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.order_line (
    shop_order_id integer NOT NULL,
    product_id integer NOT NULL,
    quantity integer NOT NULL,
    price money NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.order_line OWNER TO postgres;

--
-- TOC entry 249 (class 1259 OID 33249)
-- Name: order_status; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.order_status (
    id integer NOT NULL,
    name character varying(15) NOT NULL,
    description text
);


ALTER TABLE public.order_status OWNER TO postgres;

--
-- TOC entry 248 (class 1259 OID 33248)
-- Name: order_status_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.order_status ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.order_status_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 230 (class 1259 OID 33146)
-- Name: product; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product (
    id integer NOT NULL,
    record_id integer NOT NULL,
    format_id integer NOT NULL,
    description text,
    quantity integer NOT NULL,
    price money NOT NULL,
    inactive boolean DEFAULT false NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.product OWNER TO postgres;

--
-- TOC entry 229 (class 1259 OID 33145)
-- Name: product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.product ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 224 (class 1259 OID 33116)
-- Name: record; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.record (
    id integer NOT NULL,
    title character varying(150) NOT NULL,
    description text,
    image_url character varying(150),
    release_date date,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.record OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 33115)
-- Name: record_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.record ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.record_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 221 (class 1259 OID 33103)
-- Name: region; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.region (
    id integer NOT NULL,
    region_name character varying(50)
);


ALTER TABLE public.region OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 33102)
-- Name: region_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.region ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.region_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 247 (class 1259 OID 33239)
-- Name: review; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.review (
    id integer NOT NULL,
    user_id integer NOT NULL,
    product_id integer NOT NULL,
    rating integer NOT NULL,
    description text,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.review OWNER TO postgres;

--
-- TOC entry 246 (class 1259 OID 33238)
-- Name: review_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.review ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.review_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 244 (class 1259 OID 33226)
-- Name: shop_order; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.shop_order (
    id integer NOT NULL,
    user_id integer NOT NULL,
    total money NOT NULL,
    city character varying(50) NOT NULL,
    street character varying(150) NOT NULL,
    building character varying(10) NOT NULL,
    apartment character varying(10),
    status_id integer NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.shop_order OWNER TO postgres;

--
-- TOC entry 243 (class 1259 OID 33225)
-- Name: shop_order_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.shop_order ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.shop_order_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 241 (class 1259 OID 33213)
-- Name: shopping_cart; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.shopping_cart (
    id integer NOT NULL,
    user_id integer NOT NULL
);


ALTER TABLE public.shopping_cart OWNER TO postgres;

--
-- TOC entry 240 (class 1259 OID 33212)
-- Name: shopping_cart_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.shopping_cart ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.shopping_cart_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 242 (class 1259 OID 33218)
-- Name: shopping_cart_product; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.shopping_cart_product (
    shopping_cart_id integer NOT NULL,
    product_id integer NOT NULL,
    quantity integer,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.shopping_cart_product OWNER TO postgres;

--
-- TOC entry 239 (class 1259 OID 33193)
-- Name: track; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.track (
    id integer NOT NULL,
    title character varying NOT NULL,
    duration_seconds integer NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.track OWNER TO postgres;

--
-- TOC entry 238 (class 1259 OID 33192)
-- Name: track_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.track ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.track_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 250 (class 1259 OID 42294)
-- Name: track_product; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.track_product (
    track_id integer NOT NULL,
    product_id integer NOT NULL,
    track_order character varying(10)
);


ALTER TABLE public.track_product OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 33110)
-- Name: user_address; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.user_address (
    user_id integer NOT NULL,
    address_id integer NOT NULL,
    is_default boolean
);


ALTER TABLE public.user_address OWNER TO postgres;

--
-- TOC entry 3330 (class 2606 OID 33101)
-- Name: address address_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.address
    ADD CONSTRAINT address_pkey PRIMARY KEY (id);


--
-- TOC entry 3326 (class 2606 OID 33093)
-- Name: app_user app_user_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.app_user
    ADD CONSTRAINT app_user_email_key UNIQUE (email);


--
-- TOC entry 3340 (class 2606 OID 33134)
-- Name: artist artist_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artist
    ADD CONSTRAINT artist_pkey PRIMARY KEY (id);


--
-- TOC entry 3350 (class 2606 OID 33169)
-- Name: artist_record artist_record_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artist_record
    ADD CONSTRAINT artist_record_pkey PRIMARY KEY (artist_id, record_id);


--
-- TOC entry 3348 (class 2606 OID 33164)
-- Name: discount discount_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.discount
    ADD CONSTRAINT discount_pkey PRIMARY KEY (id);


--
-- TOC entry 3342 (class 2606 OID 33144)
-- Name: format format_format_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.format
    ADD CONSTRAINT format_format_name_key UNIQUE (format_name);


--
-- TOC entry 3344 (class 2606 OID 33142)
-- Name: format format_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.format
    ADD CONSTRAINT format_pkey PRIMARY KEY (id);


--
-- TOC entry 3358 (class 2606 OID 33191)
-- Name: genre_artist genre_artist_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre_artist
    ADD CONSTRAINT genre_artist_pkey PRIMARY KEY (genre_id, artist_id);


--
-- TOC entry 3352 (class 2606 OID 33946)
-- Name: genre genre_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre
    ADD CONSTRAINT genre_name_key UNIQUE (name);


--
-- TOC entry 3354 (class 2606 OID 33179)
-- Name: genre genre_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre
    ADD CONSTRAINT genre_pkey PRIMARY KEY (id);


--
-- TOC entry 3356 (class 2606 OID 33186)
-- Name: genre_record genre_record_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre_record
    ADD CONSTRAINT genre_record_pkey PRIMARY KEY (genre_id, record_id);


--
-- TOC entry 3368 (class 2606 OID 33237)
-- Name: order_line order_line_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_line
    ADD CONSTRAINT order_line_pkey PRIMARY KEY (shop_order_id, product_id);


--
-- TOC entry 3372 (class 2606 OID 33255)
-- Name: order_status order_status_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_status
    ADD CONSTRAINT order_status_pkey PRIMARY KEY (id);


--
-- TOC entry 3346 (class 2606 OID 33155)
-- Name: product product_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product
    ADD CONSTRAINT product_pkey PRIMARY KEY (id);


--
-- TOC entry 3338 (class 2606 OID 33124)
-- Name: record record_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.record
    ADD CONSTRAINT record_pkey PRIMARY KEY (id);


--
-- TOC entry 3332 (class 2606 OID 33107)
-- Name: region region_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.region
    ADD CONSTRAINT region_pkey PRIMARY KEY (id);


--
-- TOC entry 3334 (class 2606 OID 33109)
-- Name: region region_region_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.region
    ADD CONSTRAINT region_region_name_key UNIQUE (region_name);


--
-- TOC entry 3370 (class 2606 OID 33247)
-- Name: review review_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.review
    ADD CONSTRAINT review_pkey PRIMARY KEY (id);


--
-- TOC entry 3366 (class 2606 OID 33231)
-- Name: shop_order shop_order_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shop_order
    ADD CONSTRAINT shop_order_pkey PRIMARY KEY (id);


--
-- TOC entry 3362 (class 2606 OID 33217)
-- Name: shopping_cart shopping_cart_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shopping_cart
    ADD CONSTRAINT shopping_cart_pkey PRIMARY KEY (id);


--
-- TOC entry 3364 (class 2606 OID 33224)
-- Name: shopping_cart_product shopping_cart_product_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shopping_cart_product
    ADD CONSTRAINT shopping_cart_product_pkey PRIMARY KEY (shopping_cart_id, product_id);


--
-- TOC entry 3360 (class 2606 OID 33201)
-- Name: track track_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.track
    ADD CONSTRAINT track_pkey PRIMARY KEY (id);


--
-- TOC entry 3374 (class 2606 OID 42298)
-- Name: track_product track_product_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.track_product
    ADD CONSTRAINT track_product_pkey PRIMARY KEY (track_id, product_id);


--
-- TOC entry 3336 (class 2606 OID 33114)
-- Name: user_address user_address_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_address
    ADD CONSTRAINT user_address_pkey PRIMARY KEY (user_id, address_id);


--
-- TOC entry 3328 (class 2606 OID 33091)
-- Name: app_user user_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.app_user
    ADD CONSTRAINT user_pkey PRIMARY KEY (id);


--
-- TOC entry 3399 (class 2620 OID 33382)
-- Name: address address_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER address_updated_at BEFORE UPDATE ON public.address FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3402 (class 2620 OID 33385)
-- Name: artist artist_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER artist_updated_at BEFORE UPDATE ON public.artist FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3405 (class 2620 OID 33388)
-- Name: discount discount_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER discount_updated_at BEFORE UPDATE ON public.discount FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3403 (class 2620 OID 33386)
-- Name: format format_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER format_updated_at BEFORE UPDATE ON public.format FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3406 (class 2620 OID 33389)
-- Name: genre genre_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER genre_updated_at BEFORE UPDATE ON public.genre FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3404 (class 2620 OID 33387)
-- Name: product product_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER product_updated_at BEFORE UPDATE ON public.product FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3401 (class 2620 OID 33384)
-- Name: record record_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER record_updated_at BEFORE UPDATE ON public.record FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3400 (class 2620 OID 33383)
-- Name: region region_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER region_updated_at BEFORE UPDATE ON public.region FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3409 (class 2620 OID 33392)
-- Name: review review_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER review_updated_at BEFORE UPDATE ON public.review FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3408 (class 2620 OID 33391)
-- Name: shopping_cart_product shopping_cart_product_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER shopping_cart_product_updated_at BEFORE UPDATE ON public.shopping_cart_product FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3407 (class 2620 OID 33390)
-- Name: track track_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER track_updated_at BEFORE UPDATE ON public.track FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3398 (class 2620 OID 33381)
-- Name: app_user user_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER user_updated_at BEFORE UPDATE ON public.app_user FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3375 (class 2606 OID 33256)
-- Name: address address_region_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.address
    ADD CONSTRAINT address_region_id_fkey FOREIGN KEY (region_id) REFERENCES public.region(id) NOT VALID;


--
-- TOC entry 3381 (class 2606 OID 33286)
-- Name: artist_record artist_record_artist_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artist_record
    ADD CONSTRAINT artist_record_artist_id_fkey FOREIGN KEY (artist_id) REFERENCES public.artist(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3382 (class 2606 OID 33291)
-- Name: artist_record artist_record_record_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artist_record
    ADD CONSTRAINT artist_record_record_id_fkey FOREIGN KEY (record_id) REFERENCES public.record(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3380 (class 2606 OID 33281)
-- Name: discount discount_product_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.discount
    ADD CONSTRAINT discount_product_id_fkey FOREIGN KEY (product_id) REFERENCES public.product(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3385 (class 2606 OID 33311)
-- Name: genre_artist genre_artist_artist_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre_artist
    ADD CONSTRAINT genre_artist_artist_id_fkey FOREIGN KEY (artist_id) REFERENCES public.artist(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3386 (class 2606 OID 33306)
-- Name: genre_artist genre_artist_genre_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre_artist
    ADD CONSTRAINT genre_artist_genre_id_fkey FOREIGN KEY (genre_id) REFERENCES public.genre(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3383 (class 2606 OID 33296)
-- Name: genre_record genre_record_genre_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre_record
    ADD CONSTRAINT genre_record_genre_id_fkey FOREIGN KEY (genre_id) REFERENCES public.genre(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3384 (class 2606 OID 33301)
-- Name: genre_record genre_record_record_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre_record
    ADD CONSTRAINT genre_record_record_id_fkey FOREIGN KEY (record_id) REFERENCES public.record(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3392 (class 2606 OID 33366)
-- Name: order_line order_line_product_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_line
    ADD CONSTRAINT order_line_product_id_fkey FOREIGN KEY (product_id) REFERENCES public.product(id) ON UPDATE CASCADE ON DELETE SET NULL NOT VALID;


--
-- TOC entry 3393 (class 2606 OID 33361)
-- Name: order_line order_line_shop_order_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_line
    ADD CONSTRAINT order_line_shop_order_id_fkey FOREIGN KEY (shop_order_id) REFERENCES public.shop_order(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3378 (class 2606 OID 33276)
-- Name: product product_format_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product
    ADD CONSTRAINT product_format_id_fkey FOREIGN KEY (format_id) REFERENCES public.format(id) NOT VALID;


--
-- TOC entry 3379 (class 2606 OID 33271)
-- Name: product product_record_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product
    ADD CONSTRAINT product_record_id_fkey FOREIGN KEY (record_id) REFERENCES public.record(id) NOT VALID;


--
-- TOC entry 3394 (class 2606 OID 33376)
-- Name: review review_product_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.review
    ADD CONSTRAINT review_product_id_fkey FOREIGN KEY (product_id) REFERENCES public.product(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3395 (class 2606 OID 33371)
-- Name: review review_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.review
    ADD CONSTRAINT review_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.app_user(id) ON UPDATE CASCADE ON DELETE SET NULL NOT VALID;


--
-- TOC entry 3390 (class 2606 OID 33356)
-- Name: shop_order shop_order_status_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shop_order
    ADD CONSTRAINT shop_order_status_id_fkey FOREIGN KEY (status_id) REFERENCES public.order_status(id) NOT VALID;


--
-- TOC entry 3391 (class 2606 OID 33351)
-- Name: shop_order shop_order_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shop_order
    ADD CONSTRAINT shop_order_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.app_user(id) ON UPDATE CASCADE ON DELETE SET NULL NOT VALID;


--
-- TOC entry 3388 (class 2606 OID 33346)
-- Name: shopping_cart_product shopping_cart_product_product_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shopping_cart_product
    ADD CONSTRAINT shopping_cart_product_product_id_fkey FOREIGN KEY (product_id) REFERENCES public.product(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3389 (class 2606 OID 33341)
-- Name: shopping_cart_product shopping_cart_product_shopping_cart_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shopping_cart_product
    ADD CONSTRAINT shopping_cart_product_shopping_cart_id_fkey FOREIGN KEY (shopping_cart_id) REFERENCES public.shopping_cart(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3387 (class 2606 OID 33336)
-- Name: shopping_cart shopping_cart_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shopping_cart
    ADD CONSTRAINT shopping_cart_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.app_user(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3396 (class 2606 OID 42299)
-- Name: track_product track_product_product_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.track_product
    ADD CONSTRAINT track_product_product_id_fkey FOREIGN KEY (product_id) REFERENCES public.product(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3397 (class 2606 OID 42304)
-- Name: track_product track_product_track_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.track_product
    ADD CONSTRAINT track_product_track_id_fkey FOREIGN KEY (track_id) REFERENCES public.track(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3376 (class 2606 OID 33266)
-- Name: user_address user_address_address_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_address
    ADD CONSTRAINT user_address_address_id_fkey FOREIGN KEY (address_id) REFERENCES public.address(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3377 (class 2606 OID 33261)
-- Name: user_address user_address_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_address
    ADD CONSTRAINT user_address_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.app_user(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- PostgreSQL database dump complete
--

