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
-- TOC entry 3619 (class 0 OID 0)
-- Dependencies: 2
-- Name: EXTENSION moddatetime; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION moddatetime IS 'functions for tracking last modification time';


--
-- TOC entry 956 (class 1247 OID 99677)
-- Name: order_status; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.order_status AS ENUM (
    'Pending',
    'Paid',
    'Processing',
    'Shipped',
    'Delivered',
    'Canceled'
);


ALTER TYPE public.order_status OWNER TO postgres;

--
-- TOC entry 283 (class 1255 OID 107862)
-- Name: create_order(integer, character varying, character varying, character varying, character varying, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.create_order(IN _user_id integer, IN _city character varying, IN _street character varying, IN _building character varying, IN _apartment character varying, IN _region character varying)
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
DECLARE
    _products_count integer;
    _order_id integer;
    _product_row record;
BEGIN

    -- Check if the user has products in the shopping cart
    SELECT COUNT(*) INTO _products_count
    FROM shopping_cart_product scp
    JOIN shopping_cart sc ON sc.id = scp.shopping_cart_id
    WHERE sc.user_id = _user_id;

    IF _products_count < 1 THEN
        RAISE EXCEPTION 'Must have products in cart to place order' USING ERRCODE = 'P0001';
    END IF;

    -- Check if there is enough inventory for each product
    FOR _product_row IN
        SELECT scp.product_id, scp.quantity
        FROM shopping_cart_product scp
        JOIN shopping_cart sc ON sc.id = scp.shopping_cart_id
        WHERE sc.user_id = _user_id
    LOOP
        IF (SELECT COALESCE(SUM(quantity), 0) FROM inventory WHERE inventory.product_id = _product_row.product_id) < _product_row.quantity THEN
            RAISE EXCEPTION 'Not enough quantity of product % available in storage', _product_row.product_id USING ERRCODE = 'P0002';
        END IF;
    END LOOP;

    -- Create the order
    INSERT INTO shop_order (user_id, city, street, building, apartment, region, status)
    VALUES (
        _user_id,
        _city,
        _street,
        _building,
        _apartment,
        _region,
        'Pending'
    )
    RETURNING id INTO _order_id;

    -- Create order lines
    INSERT INTO order_line (shop_order_id, product_id, quantity, price)
    SELECT _order_id, p.id, scp.quantity, p.price
    FROM shopping_cart_product scp
    JOIN shopping_cart sc ON sc.id = scp.shopping_cart_id
    JOIN product p ON p.id = scp.product_id
    WHERE sc.user_id = _user_id;

    -- Clean up the shopping cart
    DELETE FROM shopping_cart_product
    USING shopping_cart
    WHERE shopping_cart.id = shopping_cart_product.shopping_cart_id
      AND shopping_cart.user_id = _user_id;
END;
$$;


ALTER PROCEDURE public.create_order(IN _user_id integer, IN _city character varying, IN _street character varying, IN _building character varying, IN _apartment character varying, IN _region character varying) OWNER TO postgres;

--
-- TOC entry 257 (class 1255 OID 107843)
-- Name: generate_reorder_notifications(); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.generate_reorder_notifications()
    LANGUAGE plpgsql
    AS $$
DECLARE
    product_id INTEGER;
    product_quantity INTEGER;
    product_restock_level INTEGER;
	product_record RECORD;
BEGIN
    FOR product_record IN
        SELECT p.id, i.quantity, i.restock_level
        FROM product p
        JOIN inventory i ON p.id = i.product_id
    LOOP
        product_id := product_record.id;
        product_quantity := product_record.quantity;
        product_restock_level := product_record.restock_level;

        IF product_quantity < product_restock_level THEN
            RAISE NOTICE 'Product % requires reorder. Quantity: %, Restock Level: %', product_id, product_quantity, product_restock_level;
        END IF;
    END LOOP;
END;
$$;


ALTER PROCEDURE public.generate_reorder_notifications() OWNER TO postgres;

--
-- TOC entry 284 (class 1255 OID 148801)
-- Name: get_average_order_value(text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_average_order_value(granularity text) RETURNS TABLE(year integer, period integer, average_order_value numeric)
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF granularity = 'monthly' THEN
        RETURN QUERY
        SELECT
            CAST(date_part('year', so.created_at::date) AS INTEGER) AS year,
            CAST(date_part('month', so.created_at::date) AS INTEGER) AS period,
            COALESCE(SUM(ol.quantity * ol.price) / COUNT(DISTINCT so.id), 0) AS average_order_value
        FROM
            shop_order so
        JOIN
            order_line ol ON ol.shop_order_id = so.id
        GROUP BY
            year, period
        ORDER BY
            year, period;
    ELSIF granularity = 'yearly' THEN
        RETURN QUERY
        SELECT
            CAST(date_part('year', so.created_at::date) AS INTEGER) AS year,
            1 AS period, -- Set period to a default value for yearly data
            COALESCE(SUM(ol.quantity * ol.price) / COUNT(DISTINCT so.id), 0) AS average_order_value
        FROM
            shop_order so
        JOIN
            order_line ol ON ol.shop_order_id = so.id
        GROUP BY
            year
        ORDER BY
            year;
    ELSIF granularity = 'weekly' THEN
        RETURN QUERY
        SELECT
            CAST(date_part('year', so.created_at::date) AS INTEGER) AS year,
            CAST(date_part('week', so.created_at::date) AS INTEGER) AS period,
            COALESCE(SUM(ol.quantity * ol.price) / COUNT(DISTINCT so.id), 0) AS average_order_value
        FROM
            shop_order so
        JOIN
            order_line ol ON ol.shop_order_id = so.id
        GROUP BY
            year, period
        ORDER BY
            year, period;
    ELSE
        RAISE EXCEPTION 'Invalid granularity: %', granularity;
    END IF;
END;
$$;


ALTER FUNCTION public.get_average_order_value(granularity text) OWNER TO postgres;

--
-- TOC entry 281 (class 1255 OID 148791)
-- Name: get_financial_stats(text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_financial_stats(granularity text) RETURNS TABLE(year integer, period integer, revenue numeric, expenses numeric, profit numeric)
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF granularity = 'monthly' THEN
        RETURN QUERY
        WITH revenue AS (
            SELECT
                CAST(date_part('year', so.created_at::date) AS INTEGER) AS year,
                CAST(date_part('month', so.created_at::date) AS INTEGER) AS period,
                SUM(ol.quantity * ol.price) AS total_revenue
            FROM
                shop_order so
            JOIN
                order_line ol ON ol.shop_order_id = so.id
            WHERE
                so.status = 'Shipped' OR so.status = 'Delivered'
            GROUP BY
                year, period
        ),
        expenses AS (
            SELECT
                CAST(date_part('year', created_at::date) AS INTEGER) AS year,
                CAST(date_part('month', created_at::date) AS INTEGER) AS period,
                SUM(total) AS total_expenses
            FROM
                purchase_order
            GROUP BY
                year, period
        )
        SELECT
            r.year,
            r.period,
            COALESCE(r.total_revenue, 0),
            COALESCE(e.total_expenses, 0),
            COALESCE(r.total_revenue, 0) - COALESCE(e.total_expenses, 0) AS profit
        FROM
            revenue r
        LEFT JOIN
            expenses e ON r.year = e.year AND r.period = e.period
        ORDER BY
            r.year, r.period;
    ELSIF granularity = 'yearly' THEN
        RETURN QUERY
        WITH revenue AS (
            SELECT
                CAST(date_part('year', so.created_at::date) AS INTEGER) AS year,
                SUM(ol.quantity * ol.price) AS total_revenue
            FROM
                shop_order so
            JOIN
                order_line ol ON ol.shop_order_id = so.id
            WHERE
                so.status = 'Shipped' OR so.status = 'Delivered'
            GROUP BY
                year
        ),
        expenses AS (
            SELECT
                CAST(date_part('year', created_at::date) AS INTEGER) AS year,
                SUM(total) AS total_expenses
            FROM
                purchase_order
            GROUP BY
                year
        )
        SELECT
            r.year,
            1 AS period, -- Set period to a default value for yearly data
            COALESCE(r.total_revenue, 0),
            COALESCE(e.total_expenses, 0),
            COALESCE(r.total_revenue, 0) - COALESCE(e.total_expenses, 0) AS profit
        FROM
            revenue r
        LEFT JOIN
            expenses e ON r.year = e.year
        ORDER BY
            r.year;
    ELSIF granularity = 'weekly' THEN
        RETURN QUERY
        WITH revenue AS (
            SELECT
                CAST(date_part('year', so.created_at::date) AS INTEGER) AS year,
                CAST(date_part('week', so.created_at::date) AS INTEGER) AS period,
                SUM(ol.quantity * ol.price) AS total_revenue
            FROM
                shop_order so
            JOIN
                order_line ol ON ol.shop_order_id = so.id
            WHERE
                so.status = 'Shipped' OR so.status = 'Delivered'
            GROUP BY
                year, period
        ),
        expenses AS (
            SELECT
                CAST(date_part('year', created_at::date) AS INTEGER) AS year,
                CAST(date_part('week', created_at::date) AS INTEGER) AS period,
                SUM(total) AS total_expenses
            FROM
                purchase_order
            GROUP BY
                year, period
        )
        SELECT
            r.year,
            r.period,
            COALESCE(r.total_revenue, 0),
            COALESCE(e.total_expenses, 0),
            COALESCE(r.total_revenue, 0) - COALESCE(e.total_expenses, 0) AS profit
        FROM
            revenue r
        LEFT JOIN
            expenses e ON r.year = e.year AND r.period = e.period
        ORDER BY
            r.year, r.period;
    ELSE
        RAISE EXCEPTION 'Invalid granularity: %', granularity;
    END IF;
END;
$$;


ALTER FUNCTION public.get_financial_stats(granularity text) OWNER TO postgres;

--
-- TOC entry 282 (class 1255 OID 124321)
-- Name: get_financial_summary(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_financial_summary() RETURNS TABLE(total_orders integer, total_income numeric, total_expenses numeric, net_income numeric)
    LANGUAGE plpgsql
    AS $$
DECLARE
    total_orders_count INT;
    total_income_amount DECIMAL;
    total_expenses_amount DECIMAL;
BEGIN
    -- Calculate total orders count
    SELECT COUNT(*) INTO total_orders_count FROM shop_order;
    
    -- Calculate total income
    SELECT SUM(ol.quantity * ol.price) INTO total_income_amount
    FROM shop_order so
    JOIN order_line ol ON ol.shop_order_id = so.id
    WHERE so.status = 'Shipped' OR so.status = 'Delivered';
    
    -- Calculate total expenses
    SELECT SUM(total) INTO total_expenses_amount
    FROM purchase_order;
    
    -- Calculate net income
    net_income := total_income_amount - total_expenses_amount;
    
    -- Return the results
    RETURN QUERY SELECT total_orders_count, total_income_amount, total_expenses_amount, net_income;
END;
$$;


ALTER FUNCTION public.get_financial_summary() OWNER TO postgres;

--
-- TOC entry 259 (class 1255 OID 148802)
-- Name: get_order_count_by_product(integer, text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_order_count_by_product(_product_id integer, granularity text) RETURNS TABLE(year integer, period integer, num_orders bigint)
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF granularity = 'monthly' THEN
        RETURN QUERY
        SELECT
            CAST(date_part('year', so.created_at::date) AS INTEGER) AS year,
            CAST(date_part('month', so.created_at::date) AS INTEGER) AS period,
            COUNT(*) AS num_orders
        FROM
            shop_order so
        JOIN
            order_line ol ON ol.shop_order_id = so.id
        WHERE
			(so.status = 'Shipped' OR so.status = 'Delivered') AND
            ol.product_id = _product_id
        GROUP BY
            year, period
        ORDER BY
            year, period;
    ELSIF granularity = 'yearly' THEN
        RETURN QUERY
        SELECT
            CAST(date_part('year', so.created_at::date) AS INTEGER) AS year,
            1 AS period, -- Set period to a default value for yearly data
            COUNT(*) AS num_orders
        FROM
            shop_order so
        JOIN
            order_line ol ON ol.shop_order_id = so.id
        WHERE
			(so.status = 'Shipped' OR so.status = 'Delivered') AND
            ol.product_id = _product_id
        GROUP BY
            year
        ORDER BY
            year;
    ELSIF granularity = 'weekly' THEN
        RETURN QUERY
        SELECT
            CAST(date_part('year', so.created_at::date) AS INTEGER) AS year,
            CAST(date_part('week', so.created_at::date) AS INTEGER) AS period,
            COUNT(*) AS num_orders
        FROM
            shop_order so
        JOIN
            order_line ol ON ol.shop_order_id = so.id
        WHERE
		(so.status = 'Shipped' OR so.status = 'Delivered') AND
            ol.product_id = _product_id
        GROUP BY
            year, period
        ORDER BY
            year, period;
    ELSE
        RAISE EXCEPTION 'Invalid granularity: %', granularity;
    END IF;
END;
$$;


ALTER FUNCTION public.get_order_count_by_product(_product_id integer, granularity text) OWNER TO postgres;

--
-- TOC entry 285 (class 1255 OID 148790)
-- Name: get_order_stats(text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_order_stats(granularity text) RETURNS TABLE(year integer, period integer, num_orders bigint)
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF granularity = 'monthly' THEN
        RETURN QUERY
        SELECT CAST(date_part('year', created_at::date) AS integer) AS year,
               CAST(date_part('month', created_at::date) AS integer) AS period,
               COUNT(id) AS num_orders
        FROM shop_order
		WHERE shop_order.status = 'Shipped' OR shop_order.status = 'Delivered'
        GROUP BY year, period
        ORDER BY year, period;
    ELSIF granularity = 'yearly' THEN
        RETURN QUERY
        SELECT CAST(date_part('year', created_at::date) AS integer) AS year,
               1 AS period, -- Set period to a default value for yearly data
               COUNT(id) AS num_orders
        FROM shop_order
		WHERE shop_order.status = 'Shipped' OR shop_order.status = 'Delivered'
        GROUP BY year
        ORDER BY year;
    ELSIF granularity = 'weekly' THEN
        RETURN QUERY
        SELECT CAST(date_part('year', created_at::date) AS integer) AS year,
               CAST(date_part('week', created_at::date) AS integer) AS period,
               COUNT(id) AS num_orders
        FROM shop_order
		WHERE shop_order.status = 'Shipped' OR shop_order.status = 'Delivered'
        GROUP BY year, period
        ORDER BY year, period;
    ELSE
        RAISE EXCEPTION 'Invalid granularity: %', granularity;
    END IF;
END;
$$;


ALTER FUNCTION public.get_order_stats(granularity text) OWNER TO postgres;

--
-- TOC entry 258 (class 1255 OID 148800)
-- Name: get_product_quantity_sold(integer, text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_product_quantity_sold(_product_id integer, granularity text) RETURNS TABLE(year integer, period integer, quantity_sold bigint)
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF granularity = 'monthly' THEN
        RETURN QUERY
        SELECT
            CAST(date_part('year', so.created_at::date) AS INTEGER) AS year,
            CAST(date_part('month', so.created_at::date) AS INTEGER) AS period,
            SUM(ol.quantity) AS quantity_sold
        FROM
            shop_order so
        JOIN
            order_line ol ON ol.shop_order_id = so.id
        WHERE
            (so.status = 'Shipped' OR so.status = 'Delivered') AND
            ol.product_id = _product_id
        GROUP BY
            year, period
        ORDER BY
            year, period;
    ELSIF granularity = 'yearly' THEN
        RETURN QUERY
        SELECT
            CAST(date_part('year', so.created_at::date) AS INTEGER) AS year,
            1 AS period, -- Set period to a default value for yearly data
            SUM(ol.quantity) AS quantity_sold
        FROM
            shop_order so
        JOIN
            order_line ol ON ol.shop_order_id = so.id
        WHERE
            (so.status = 'Shipped' OR so.status = 'Delivered') AND
            ol.product_id = _product_id
        GROUP BY
            year
        ORDER BY
            year;
    ELSIF granularity = 'weekly' THEN
        RETURN QUERY
        SELECT
            CAST(date_part('year', so.created_at::date) AS INTEGER) AS year,
            CAST(date_part('week', so.created_at::date) AS INTEGER) AS period,
            SUM(ol.quantity) AS quantity_sold
        FROM
            shop_order so
        JOIN
            order_line ol ON ol.shop_order_id = so.id
        WHERE
            (so.status = 'Shipped' OR so.status = 'Delivered') AND
            ol.product_id = _product_id
        GROUP BY
            year, period
        ORDER BY
            year, period;
    ELSE
        RAISE EXCEPTION 'Invalid granularity: %', granularity;
    END IF;
END;
$$;


ALTER FUNCTION public.get_product_quantity_sold(_product_id integer, granularity text) OWNER TO postgres;

--
-- TOC entry 279 (class 1255 OID 116192)
-- Name: get_reorder_ids(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_reorder_ids() RETURNS SETOF integer
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT p.id
    FROM product p
    JOIN inventory i ON p.id = i.product_id
    WHERE i.quantity < i.restock_level;
END;
$$;


ALTER FUNCTION public.get_reorder_ids() OWNER TO postgres;

--
-- TOC entry 270 (class 1255 OID 148806)
-- Name: get_total_orders_by_region(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_total_orders_by_region() RETURNS TABLE(region character varying, total_orders bigint)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT so.region, COUNT(*) AS total_orders
    FROM shop_order so
    GROUP BY so.region
	order by total_orders;
END;
$$;


ALTER FUNCTION public.get_total_orders_by_region() OWNER TO postgres;

--
-- TOC entry 278 (class 1255 OID 165218)
-- Name: insert_log(integer, character varying, text); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.insert_log(IN p_user_id integer, IN p_action_type character varying, IN p_description text)
    LANGUAGE plpgsql
    AS $$
BEGIN
    INSERT INTO public.log (user_id, action_type, description)
    VALUES (p_user_id, p_action_type, p_description);
END;
$$;


ALTER PROCEDURE public.insert_log(IN p_user_id integer, IN p_action_type character varying, IN p_description text) OWNER TO postgres;

--
-- TOC entry 280 (class 1255 OID 116270)
-- Name: purchase_order_update_product_storage(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.purchase_order_update_product_storage() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE inventory
    SET quantity = quantity + NEW.quantity
    WHERE inventory.product_id = NEW.product_id;

    RETURN NEW;
END;
$$;


ALTER FUNCTION public.purchase_order_update_product_storage() OWNER TO postgres;

--
-- TOC entry 256 (class 1255 OID 50486)
-- Name: truncate_schema(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.truncate_schema(_schema character varying) RETURNS void
    LANGUAGE plpgsql
    AS $$
declare
    selectrow record;
begin
for selectrow in
select 'TRUNCATE TABLE ' || quote_ident(_schema) || '.' ||quote_ident(t.table_name) || ' CASCADE;' as qry 
from (
     SELECT table_name 
     FROM information_schema.tables
     WHERE table_type = 'BASE TABLE' AND table_schema = _schema
     )t
loop
execute selectrow.qry;
end loop;
end;
$$;


ALTER FUNCTION public.truncate_schema(_schema character varying) OWNER TO postgres;

--
-- TOC entry 273 (class 1255 OID 140600)
-- Name: update_inventory_on_delete(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.update_inventory_on_delete() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Calculate the change in quantity
    DECLARE
        quantity_change INT;
    BEGIN
        SELECT OLD.quantity INTO quantity_change;

        -- Update inventory quantity
        UPDATE inventory
        SET quantity = quantity - quantity_change
        WHERE product_id = OLD.product_id;
    END;

    RETURN OLD;
END;
$$;


ALTER FUNCTION public.update_inventory_on_delete() OWNER TO postgres;

--
-- TOC entry 274 (class 1255 OID 140598)
-- Name: update_inventory_on_insert(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.update_inventory_on_insert() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN 
    -- Update inventory quantity
    UPDATE inventory
    SET quantity = quantity + NEW.quantity
    WHERE product_id = NEW.product_id;

    RETURN NEW;
END;
$$;


ALTER FUNCTION public.update_inventory_on_insert() OWNER TO postgres;

--
-- TOC entry 276 (class 1255 OID 140599)
-- Name: update_inventory_on_update(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.update_inventory_on_update() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Calculate the change in quantity
    DECLARE
        quantity_change INT;
    BEGIN
        SELECT NEW.quantity - OLD.quantity INTO quantity_change;

        -- Update inventory quantity
        UPDATE inventory
        SET quantity = quantity + quantity_change
        WHERE product_id = NEW.product_id;
    END;

    RETURN NEW;
END;
$$;


ALTER FUNCTION public.update_inventory_on_update() OWNER TO postgres;

--
-- TOC entry 275 (class 1255 OID 107845)
-- Name: update_order_status(integer, public.order_status); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.update_order_status(IN order_id integer, IN new_status public.order_status)
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Update the status of the specified order
    UPDATE shop_order
    SET status = new_status
    WHERE id = order_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Order with ID % not found', order_id USING ERRCODE = 'P0001';
    END IF;
END;
$$;


ALTER PROCEDURE public.update_order_status(IN order_id integer, IN new_status public.order_status) OWNER TO postgres;

--
-- TOC entry 272 (class 1255 OID 75062)
-- Name: update_product_storage(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.update_product_storage() RETURNS trigger
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
BEGIN
    UPDATE inventory
    SET quantity = quantity - NEW.quantity
    WHERE inventory.product_id = NEW.product_id;
    
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.update_product_storage() OWNER TO postgres;

--
-- TOC entry 277 (class 1255 OID 107855)
-- Name: update_stock_movement(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.update_stock_movement() RETURNS trigger
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
BEGIN
	IF NEW.status = 'Shipped' THEN
		UPDATE inventory i
    	SET quantity = i.quantity - ol.quantity
		from order_line ol
    	WHERE i.product_id = ol.product_id and ol.shop_order_id = new.id;
    END IF;
	
    IF NEW.status = 'Canceled' AND OLD.status = 'Shipped' THEN
		UPDATE inventory i
    	SET quantity = i.quantity + ol.quantity
		from order_line ol
    	WHERE i.product_id = ol.product_id and ol.shop_order_id = new.id;
    END IF;
    
    RETURN NULL; -- We don't need to modify the original data, so we return NULL
END;
$$;


ALTER FUNCTION public.update_stock_movement() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 254 (class 1259 OID 165860)
-- Name: label; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.label (
    id integer NOT NULL,
    name character varying(100) NOT NULL
);


ALTER TABLE public.label OWNER TO postgres;

--
-- TOC entry 253 (class 1259 OID 165859)
-- Name: Label_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.label ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Label_id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


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
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    user_id integer NOT NULL,
    region character varying(50)
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
    role_id integer NOT NULL,
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
-- TOC entry 223 (class 1259 OID 33126)
-- Name: artist; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.artist (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    description text,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.artist OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 33125)
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
-- TOC entry 228 (class 1259 OID 33165)
-- Name: artist_record; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.artist_record (
    artist_id integer NOT NULL,
    record_id integer NOT NULL
);


ALTER TABLE public.artist_record OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 33136)
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
-- TOC entry 224 (class 1259 OID 33135)
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
-- TOC entry 230 (class 1259 OID 33171)
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
-- TOC entry 229 (class 1259 OID 33170)
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
-- TOC entry 231 (class 1259 OID 33182)
-- Name: genre_record; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.genre_record (
    genre_id integer NOT NULL,
    record_id integer NOT NULL
);


ALTER TABLE public.genre_record OWNER TO postgres;

--
-- TOC entry 243 (class 1259 OID 91447)
-- Name: inventory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.inventory (
    id integer NOT NULL,
    product_id integer NOT NULL,
    quantity integer NOT NULL,
    location character varying(255) NOT NULL,
    restock_level integer,
    CONSTRAINT positive_quantity CHECK ((quantity >= 0))
);


ALTER TABLE public.inventory OWNER TO postgres;

--
-- TOC entry 242 (class 1259 OID 91446)
-- Name: inventory_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.inventory_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.inventory_id_seq OWNER TO postgres;

--
-- TOC entry 3631 (class 0 OID 0)
-- Dependencies: 242
-- Name: inventory_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.inventory_id_seq OWNED BY public.inventory.id;


--
-- TOC entry 250 (class 1259 OID 107841)
-- Name: inventory_id_seq1; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.inventory ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.inventory_id_seq1
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 252 (class 1259 OID 165205)
-- Name: log; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.log (
    id integer NOT NULL,
    user_id integer,
    action_type character varying(50) NOT NULL,
    "timestamp" timestamp with time zone DEFAULT now() NOT NULL,
    description text
);


ALTER TABLE public.log OWNER TO postgres;

--
-- TOC entry 251 (class 1259 OID 165204)
-- Name: log_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.log ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.log_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 237 (class 1259 OID 33232)
-- Name: order_line; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.order_line (
    shop_order_id integer NOT NULL,
    product_id integer NOT NULL,
    quantity integer NOT NULL,
    price numeric(12,2) NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.order_line OWNER TO postgres;

--
-- TOC entry 227 (class 1259 OID 33146)
-- Name: product; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product (
    id integer NOT NULL,
    record_id integer NOT NULL,
    format_id integer NOT NULL,
    description text,
    price numeric(12,2) NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    image_url character varying(512),
    label_id integer NOT NULL
);


ALTER TABLE public.product OWNER TO postgres;

--
-- TOC entry 226 (class 1259 OID 33145)
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
-- TOC entry 245 (class 1259 OID 99639)
-- Name: purchase_order; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.purchase_order (
    id integer NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    total numeric(12,2) NOT NULL,
    supplier_id integer NOT NULL
);


ALTER TABLE public.purchase_order OWNER TO postgres;

--
-- TOC entry 244 (class 1259 OID 99638)
-- Name: purchase_order_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.purchase_order ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.purchase_order_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 247 (class 1259 OID 99646)
-- Name: purchase_order_line; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.purchase_order_line (
    id integer NOT NULL,
    product_id integer,
    quantity integer NOT NULL,
    purchase_order_id integer NOT NULL
);


ALTER TABLE public.purchase_order_line OWNER TO postgres;

--
-- TOC entry 246 (class 1259 OID 99645)
-- Name: purchase_order_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.purchase_order_line ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.purchase_order_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 221 (class 1259 OID 33116)
-- Name: record; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.record (
    id integer NOT NULL,
    title character varying(150) NOT NULL,
    description text,
    release_date date,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.record OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 33115)
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
-- TOC entry 239 (class 1259 OID 33239)
-- Name: review; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.review (
    id integer NOT NULL,
    user_id integer,
    product_id integer NOT NULL,
    rating integer NOT NULL,
    description text,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.review OWNER TO postgres;

--
-- TOC entry 238 (class 1259 OID 33238)
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
-- TOC entry 241 (class 1259 OID 58679)
-- Name: role; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.role (
    id integer NOT NULL,
    role_name character varying(20) NOT NULL
);


ALTER TABLE public.role OWNER TO postgres;

--
-- TOC entry 240 (class 1259 OID 58678)
-- Name: role_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.role ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.role_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 236 (class 1259 OID 33226)
-- Name: shop_order; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.shop_order (
    id integer NOT NULL,
    user_id integer,
    city character varying(50) NOT NULL,
    street character varying(150) NOT NULL,
    building character varying(10) NOT NULL,
    apartment character varying(10),
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    status public.order_status DEFAULT 'Pending'::public.order_status NOT NULL,
    region character varying(50) NOT NULL
);


ALTER TABLE public.shop_order OWNER TO postgres;

--
-- TOC entry 235 (class 1259 OID 33225)
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
-- TOC entry 233 (class 1259 OID 33213)
-- Name: shopping_cart; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.shopping_cart (
    id integer NOT NULL,
    user_id integer NOT NULL
);


ALTER TABLE public.shopping_cart OWNER TO postgres;

--
-- TOC entry 232 (class 1259 OID 33212)
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
-- TOC entry 234 (class 1259 OID 33218)
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
-- TOC entry 249 (class 1259 OID 99664)
-- Name: supplier; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.supplier (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    phone character varying(20) NOT NULL,
    address character varying(1024) NOT NULL
);


ALTER TABLE public.supplier OWNER TO postgres;

--
-- TOC entry 248 (class 1259 OID 99663)
-- Name: supplier_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.supplier ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.supplier_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 3432 (class 2606 OID 165866)
-- Name: label Label_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.label
    ADD CONSTRAINT "Label_name_key" UNIQUE (name);


--
-- TOC entry 3434 (class 2606 OID 165864)
-- Name: label Label_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.label
    ADD CONSTRAINT "Label_pkey" PRIMARY KEY (id);


--
-- TOC entry 3359 (class 2606 OID 33101)
-- Name: address address_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.address
    ADD CONSTRAINT address_pkey PRIMARY KEY (id);


--
-- TOC entry 3353 (class 2606 OID 33093)
-- Name: app_user app_user_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.app_user
    ADD CONSTRAINT app_user_email_key UNIQUE (email);


--
-- TOC entry 3367 (class 2606 OID 33134)
-- Name: artist artist_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artist
    ADD CONSTRAINT artist_pkey PRIMARY KEY (id);


--
-- TOC entry 3380 (class 2606 OID 33169)
-- Name: artist_record artist_record_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artist_record
    ADD CONSTRAINT artist_record_pkey PRIMARY KEY (artist_id, record_id);


--
-- TOC entry 3369 (class 2606 OID 33144)
-- Name: format format_format_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.format
    ADD CONSTRAINT format_format_name_key UNIQUE (format_name);


--
-- TOC entry 3371 (class 2606 OID 33142)
-- Name: format format_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.format
    ADD CONSTRAINT format_pkey PRIMARY KEY (id);


--
-- TOC entry 3383 (class 2606 OID 33946)
-- Name: genre genre_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre
    ADD CONSTRAINT genre_name_key UNIQUE (name);


--
-- TOC entry 3385 (class 2606 OID 33179)
-- Name: genre genre_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre
    ADD CONSTRAINT genre_pkey PRIMARY KEY (id);


--
-- TOC entry 3388 (class 2606 OID 33186)
-- Name: genre_record genre_record_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre_record
    ADD CONSTRAINT genre_record_pkey PRIMARY KEY (genre_id, record_id);


--
-- TOC entry 3413 (class 2606 OID 91453)
-- Name: inventory inventory_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.inventory
    ADD CONSTRAINT inventory_pkey PRIMARY KEY (id);


--
-- TOC entry 3416 (class 2606 OID 140612)
-- Name: inventory inventory_product_id_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.inventory
    ADD CONSTRAINT inventory_product_id_key UNIQUE (product_id);


--
-- TOC entry 3430 (class 2606 OID 165212)
-- Name: log log_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.log
    ADD CONSTRAINT log_pkey PRIMARY KEY (id);


--
-- TOC entry 3402 (class 2606 OID 33237)
-- Name: order_line order_line_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_line
    ADD CONSTRAINT order_line_pkey PRIMARY KEY (shop_order_id, product_id);


--
-- TOC entry 3351 (class 2606 OID 140609)
-- Name: purchase_order_line positive_quantity; Type: CHECK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE public.purchase_order_line
    ADD CONSTRAINT positive_quantity CHECK ((quantity > 0)) NOT VALID;


--
-- TOC entry 3348 (class 2606 OID 140610)
-- Name: order_line positive_quantity; Type: CHECK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE public.order_line
    ADD CONSTRAINT positive_quantity CHECK ((quantity > 0)) NOT VALID;


--
-- TOC entry 3375 (class 2606 OID 33155)
-- Name: product product_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product
    ADD CONSTRAINT product_pkey PRIMARY KEY (id);


--
-- TOC entry 3422 (class 2606 OID 116264)
-- Name: purchase_order_line purchase_order_line_product_id_id_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.purchase_order_line
    ADD CONSTRAINT purchase_order_line_product_id_id_key UNIQUE (product_id, id);


--
-- TOC entry 3419 (class 2606 OID 99644)
-- Name: purchase_order purchase_order_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.purchase_order
    ADD CONSTRAINT purchase_order_pkey PRIMARY KEY (id);


--
-- TOC entry 3426 (class 2606 OID 99650)
-- Name: purchase_order_line purchase_order_product_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.purchase_order_line
    ADD CONSTRAINT purchase_order_product_pkey PRIMARY KEY (id);


--
-- TOC entry 3362 (class 2606 OID 33124)
-- Name: record record_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.record
    ADD CONSTRAINT record_pkey PRIMARY KEY (id);


--
-- TOC entry 3407 (class 2606 OID 33247)
-- Name: review review_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.review
    ADD CONSTRAINT review_pkey PRIMARY KEY (id);


--
-- TOC entry 3349 (class 2606 OID 75064)
-- Name: review review_rating_chk; Type: CHECK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE public.review
    ADD CONSTRAINT review_rating_chk CHECK (((rating >= 1) AND (rating <= 5))) NOT VALID;


--
-- TOC entry 3411 (class 2606 OID 58683)
-- Name: role role_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.role
    ADD CONSTRAINT role_pkey PRIMARY KEY (id);


--
-- TOC entry 3399 (class 2606 OID 33231)
-- Name: shop_order shop_order_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shop_order
    ADD CONSTRAINT shop_order_pkey PRIMARY KEY (id);


--
-- TOC entry 3391 (class 2606 OID 33217)
-- Name: shopping_cart shopping_cart_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shopping_cart
    ADD CONSTRAINT shopping_cart_pkey PRIMARY KEY (id);


--
-- TOC entry 3394 (class 2606 OID 33224)
-- Name: shopping_cart_product shopping_cart_product_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shopping_cart_product
    ADD CONSTRAINT shopping_cart_product_pkey PRIMARY KEY (shopping_cart_id, product_id);


--
-- TOC entry 3428 (class 2606 OID 99670)
-- Name: supplier supplier_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supplier
    ADD CONSTRAINT supplier_pkey PRIMARY KEY (id);


--
-- TOC entry 3357 (class 2606 OID 33091)
-- Name: app_user user_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.app_user
    ADD CONSTRAINT user_pkey PRIMARY KEY (id);


--
-- TOC entry 3360 (class 1259 OID 165174)
-- Name: address_user_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX address_user_id_idx ON public.address USING hash (user_id);


--
-- TOC entry 3354 (class 1259 OID 165175)
-- Name: app_user_first_name_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX app_user_first_name_idx ON public.app_user USING btree (first_name) WITH (deduplicate_items='true');


--
-- TOC entry 3355 (class 1259 OID 165176)
-- Name: app_user_role_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX app_user_role_id_idx ON public.app_user USING hash (role_id);


--
-- TOC entry 3365 (class 1259 OID 165177)
-- Name: artist_name_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX artist_name_idx ON public.artist USING btree (name) WITH (deduplicate_items='true');


--
-- TOC entry 3378 (class 1259 OID 165178)
-- Name: artist_record_artist_id_record_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX artist_record_artist_id_record_id_idx ON public.artist_record USING btree (artist_id, record_id) WITH (deduplicate_items='true');


--
-- TOC entry 3381 (class 1259 OID 165179)
-- Name: artist_record_record_id_artist_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX artist_record_record_id_artist_id_idx ON public.artist_record USING btree (record_id, artist_id) WITH (deduplicate_items='true');


--
-- TOC entry 3386 (class 1259 OID 165180)
-- Name: genre_record_genre_id_record_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX genre_record_genre_id_record_id_idx ON public.genre_record USING btree (genre_id, record_id) WITH (deduplicate_items='true');


--
-- TOC entry 3389 (class 1259 OID 165181)
-- Name: genre_record_record_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX genre_record_record_id_idx ON public.genre_record USING btree (record_id) WITH (deduplicate_items='true');


--
-- TOC entry 3414 (class 1259 OID 165182)
-- Name: inventory_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX inventory_product_id_idx ON public.inventory USING hash (product_id);


--
-- TOC entry 3435 (class 1259 OID 165961)
-- Name: label_name_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX label_name_idx ON public.label USING btree (name) WITH (deduplicate_items='true');


--
-- TOC entry 3403 (class 1259 OID 165185)
-- Name: order_line_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX order_line_product_id_idx ON public.order_line USING hash (product_id);


--
-- TOC entry 3404 (class 1259 OID 165183)
-- Name: order_line_shop_order_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX order_line_shop_order_id_idx ON public.order_line USING hash (shop_order_id);


--
-- TOC entry 3372 (class 1259 OID 165187)
-- Name: product_format_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX product_format_id_idx ON public.product USING hash (format_id);


--
-- TOC entry 3373 (class 1259 OID 165967)
-- Name: product_label_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX product_label_id_idx ON public.product USING hash (label_id);


--
-- TOC entry 3376 (class 1259 OID 165199)
-- Name: product_price_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX product_price_idx ON public.product USING btree (price) WITH (deduplicate_items='true');


--
-- TOC entry 3377 (class 1259 OID 165186)
-- Name: product_record_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX product_record_id_idx ON public.product USING hash (record_id);


--
-- TOC entry 3417 (class 1259 OID 165189)
-- Name: purchase_order_created_at_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX purchase_order_created_at_idx ON public.purchase_order USING btree (created_at) WITH (deduplicate_items='true');


--
-- TOC entry 3423 (class 1259 OID 165190)
-- Name: purchase_order_line_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX purchase_order_line_product_id_idx ON public.purchase_order_line USING hash (product_id);


--
-- TOC entry 3424 (class 1259 OID 165192)
-- Name: purchase_order_line_purchase_order_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX purchase_order_line_purchase_order_id_idx ON public.purchase_order_line USING hash (purchase_order_id);


--
-- TOC entry 3420 (class 1259 OID 165188)
-- Name: purchase_order_supplier_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX purchase_order_supplier_id_idx ON public.purchase_order USING hash (supplier_id);


--
-- TOC entry 3363 (class 1259 OID 165194)
-- Name: record_release_date_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX record_release_date_idx ON public.record USING btree (release_date) WITH (deduplicate_items='true');


--
-- TOC entry 3364 (class 1259 OID 165193)
-- Name: record_title_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX record_title_idx ON public.record USING btree (title) WITH (deduplicate_items='true');


--
-- TOC entry 3405 (class 1259 OID 165197)
-- Name: review_created_at_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX review_created_at_idx ON public.review USING btree (created_at) WITH (deduplicate_items='true');


--
-- TOC entry 3408 (class 1259 OID 165196)
-- Name: review_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX review_product_id_idx ON public.review USING hash (product_id);


--
-- TOC entry 3409 (class 1259 OID 165195)
-- Name: review_user_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX review_user_id_idx ON public.review USING hash (user_id);


--
-- TOC entry 3397 (class 1259 OID 165200)
-- Name: shop_order_created_at_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX shop_order_created_at_idx ON public.shop_order USING btree (created_at) WITH (deduplicate_items='true');


--
-- TOC entry 3400 (class 1259 OID 165198)
-- Name: shop_order_user_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX shop_order_user_id_idx ON public.shop_order USING hash (user_id);


--
-- TOC entry 3395 (class 1259 OID 165203)
-- Name: shopping_cart_product_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX shopping_cart_product_product_id_idx ON public.shopping_cart_product USING hash (product_id);


--
-- TOC entry 3396 (class 1259 OID 165202)
-- Name: shopping_cart_product_shopping_cart_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX shopping_cart_product_shopping_cart_id_idx ON public.shopping_cart_product USING hash (shopping_cart_id);


--
-- TOC entry 3392 (class 1259 OID 165201)
-- Name: shopping_cart_user_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX shopping_cart_user_id_idx ON public.shopping_cart USING hash (user_id);


--
-- TOC entry 3459 (class 2620 OID 33382)
-- Name: address address_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER address_updated_at BEFORE UPDATE ON public.address FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3461 (class 2620 OID 33385)
-- Name: artist artist_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER artist_updated_at BEFORE UPDATE ON public.artist FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3462 (class 2620 OID 33386)
-- Name: format format_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER format_updated_at BEFORE UPDATE ON public.format FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3464 (class 2620 OID 33389)
-- Name: genre genre_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER genre_updated_at BEFORE UPDATE ON public.genre FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3466 (class 2620 OID 107856)
-- Name: shop_order order_status_updated_trigger; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER order_status_updated_trigger AFTER UPDATE OF status ON public.shop_order FOR EACH ROW EXECUTE FUNCTION public.update_stock_movement();


--
-- TOC entry 3463 (class 2620 OID 33387)
-- Name: product product_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER product_updated_at BEFORE UPDATE ON public.product FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3460 (class 2620 OID 33384)
-- Name: record record_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER record_updated_at BEFORE UPDATE ON public.record FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3467 (class 2620 OID 33392)
-- Name: review review_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER review_updated_at BEFORE UPDATE ON public.review FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3465 (class 2620 OID 33391)
-- Name: shopping_cart_product shopping_cart_product_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER shopping_cart_product_updated_at BEFORE UPDATE ON public.shopping_cart_product FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3468 (class 2620 OID 140608)
-- Name: purchase_order_line update_inventory_on_delete_trigger; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER update_inventory_on_delete_trigger AFTER DELETE ON public.purchase_order_line FOR EACH ROW EXECUTE FUNCTION public.update_inventory_on_delete();


--
-- TOC entry 3469 (class 2620 OID 140601)
-- Name: purchase_order_line update_inventory_on_insert_trigger; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER update_inventory_on_insert_trigger AFTER INSERT ON public.purchase_order_line FOR EACH ROW EXECUTE FUNCTION public.update_inventory_on_insert();


--
-- TOC entry 3470 (class 2620 OID 140602)
-- Name: purchase_order_line update_inventory_on_update_trigger; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER update_inventory_on_update_trigger AFTER UPDATE ON public.purchase_order_line FOR EACH ROW EXECUTE FUNCTION public.update_inventory_on_update();


--
-- TOC entry 3458 (class 2620 OID 33381)
-- Name: app_user user_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER user_updated_at BEFORE UPDATE ON public.app_user FOR EACH ROW EXECUTE FUNCTION public.moddatetime('updated_at');


--
-- TOC entry 3437 (class 2606 OID 173376)
-- Name: address address_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.address
    ADD CONSTRAINT address_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.app_user(id) ON DELETE CASCADE;


--
-- TOC entry 3436 (class 2606 OID 58684)
-- Name: app_user app_user_role_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.app_user
    ADD CONSTRAINT app_user_role_id_fkey FOREIGN KEY (role_id) REFERENCES public.role(id);


--
-- TOC entry 3441 (class 2606 OID 33286)
-- Name: artist_record artist_record_artist_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artist_record
    ADD CONSTRAINT artist_record_artist_id_fkey FOREIGN KEY (artist_id) REFERENCES public.artist(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3442 (class 2606 OID 33291)
-- Name: artist_record artist_record_record_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artist_record
    ADD CONSTRAINT artist_record_record_id_fkey FOREIGN KEY (record_id) REFERENCES public.record(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3443 (class 2606 OID 33296)
-- Name: genre_record genre_record_genre_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre_record
    ADD CONSTRAINT genre_record_genre_id_fkey FOREIGN KEY (genre_id) REFERENCES public.genre(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3444 (class 2606 OID 33301)
-- Name: genre_record genre_record_record_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre_record
    ADD CONSTRAINT genre_record_record_id_fkey FOREIGN KEY (record_id) REFERENCES public.record(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3453 (class 2606 OID 173505)
-- Name: inventory inventory_product_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.inventory
    ADD CONSTRAINT inventory_product_id_fkey FOREIGN KEY (product_id) REFERENCES public.product(id) ON DELETE CASCADE;


--
-- TOC entry 3457 (class 2606 OID 173366)
-- Name: log log_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.log
    ADD CONSTRAINT log_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.app_user(id) ON DELETE SET NULL;


--
-- TOC entry 3449 (class 2606 OID 173515)
-- Name: order_line order_line_product_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_line
    ADD CONSTRAINT order_line_product_id_fkey FOREIGN KEY (product_id) REFERENCES public.product(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- TOC entry 3450 (class 2606 OID 33361)
-- Name: order_line order_line_shop_order_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_line
    ADD CONSTRAINT order_line_shop_order_id_fkey FOREIGN KEY (shop_order_id) REFERENCES public.shop_order(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3438 (class 2606 OID 33276)
-- Name: product product_format_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product
    ADD CONSTRAINT product_format_id_fkey FOREIGN KEY (format_id) REFERENCES public.format(id) NOT VALID;


--
-- TOC entry 3439 (class 2606 OID 165962)
-- Name: product product_label_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product
    ADD CONSTRAINT product_label_id_fkey FOREIGN KEY (label_id) REFERENCES public.label(id) NOT VALID;


--
-- TOC entry 3440 (class 2606 OID 173500)
-- Name: product product_record_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product
    ADD CONSTRAINT product_record_id_fkey FOREIGN KEY (record_id) REFERENCES public.record(id) ON DELETE CASCADE;


--
-- TOC entry 3455 (class 2606 OID 140603)
-- Name: purchase_order_line purchase_order_line_purchase_order_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.purchase_order_line
    ADD CONSTRAINT purchase_order_line_purchase_order_id_fkey FOREIGN KEY (purchase_order_id) REFERENCES public.purchase_order(id) ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3456 (class 2606 OID 173510)
-- Name: purchase_order_line purchase_order_product_product_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.purchase_order_line
    ADD CONSTRAINT purchase_order_product_product_id_fkey FOREIGN KEY (product_id) REFERENCES public.product(id) ON DELETE SET NULL;


--
-- TOC entry 3454 (class 2606 OID 99671)
-- Name: purchase_order purchase_order_supplier_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.purchase_order
    ADD CONSTRAINT purchase_order_supplier_id_fkey FOREIGN KEY (supplier_id) REFERENCES public.supplier(id) NOT VALID;


--
-- TOC entry 3451 (class 2606 OID 33376)
-- Name: review review_product_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.review
    ADD CONSTRAINT review_product_id_fkey FOREIGN KEY (product_id) REFERENCES public.product(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3452 (class 2606 OID 173371)
-- Name: review review_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.review
    ADD CONSTRAINT review_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.app_user(id) ON DELETE SET NULL;


--
-- TOC entry 3448 (class 2606 OID 33351)
-- Name: shop_order shop_order_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shop_order
    ADD CONSTRAINT shop_order_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.app_user(id) ON UPDATE CASCADE ON DELETE SET NULL NOT VALID;


--
-- TOC entry 3446 (class 2606 OID 33346)
-- Name: shopping_cart_product shopping_cart_product_product_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shopping_cart_product
    ADD CONSTRAINT shopping_cart_product_product_id_fkey FOREIGN KEY (product_id) REFERENCES public.product(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3447 (class 2606 OID 33341)
-- Name: shopping_cart_product shopping_cart_product_shopping_cart_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shopping_cart_product
    ADD CONSTRAINT shopping_cart_product_shopping_cart_id_fkey FOREIGN KEY (shopping_cart_id) REFERENCES public.shopping_cart(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3445 (class 2606 OID 33336)
-- Name: shopping_cart shopping_cart_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shopping_cart
    ADD CONSTRAINT shopping_cart_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.app_user(id) ON UPDATE CASCADE ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3620 (class 0 OID 0)
-- Dependencies: 282
-- Name: FUNCTION get_financial_summary(); Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON FUNCTION public.get_financial_summary() TO "user";


--
-- TOC entry 3621 (class 0 OID 0)
-- Dependencies: 278
-- Name: PROCEDURE insert_log(IN p_user_id integer, IN p_action_type character varying, IN p_description text); Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON PROCEDURE public.insert_log(IN p_user_id integer, IN p_action_type character varying, IN p_description text) TO employee;
GRANT ALL ON PROCEDURE public.insert_log(IN p_user_id integer, IN p_action_type character varying, IN p_description text) TO guest;
GRANT ALL ON PROCEDURE public.insert_log(IN p_user_id integer, IN p_action_type character varying, IN p_description text) TO "user";


--
-- TOC entry 3622 (class 0 OID 0)
-- Dependencies: 254
-- Name: TABLE label; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.label TO employee;
GRANT SELECT ON TABLE public.label TO guest;
GRANT SELECT ON TABLE public.label TO "user";


--
-- TOC entry 3623 (class 0 OID 0)
-- Dependencies: 219
-- Name: TABLE address; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,REFERENCES,DELETE,UPDATE ON TABLE public.address TO "user";
GRANT SELECT ON TABLE public.address TO employee;


--
-- TOC entry 3624 (class 0 OID 0)
-- Dependencies: 217
-- Name: TABLE app_user; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.app_user TO "user";
GRANT SELECT ON TABLE public.app_user TO employee;
GRANT SELECT,INSERT ON TABLE public.app_user TO guest;


--
-- TOC entry 3625 (class 0 OID 0)
-- Dependencies: 223
-- Name: TABLE artist; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.artist TO "user";
GRANT SELECT,INSERT,REFERENCES,DELETE,UPDATE ON TABLE public.artist TO employee;
GRANT SELECT ON TABLE public.artist TO guest;


--
-- TOC entry 3626 (class 0 OID 0)
-- Dependencies: 228
-- Name: TABLE artist_record; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.artist_record TO "user";
GRANT SELECT,INSERT,REFERENCES,DELETE,UPDATE ON TABLE public.artist_record TO employee;
GRANT SELECT ON TABLE public.artist_record TO guest;


--
-- TOC entry 3627 (class 0 OID 0)
-- Dependencies: 225
-- Name: TABLE format; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.format TO "user";
GRANT SELECT,INSERT,REFERENCES,DELETE,UPDATE ON TABLE public.format TO employee;
GRANT SELECT ON TABLE public.format TO guest;


--
-- TOC entry 3628 (class 0 OID 0)
-- Dependencies: 230
-- Name: TABLE genre; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.genre TO "user";
GRANT SELECT,INSERT,REFERENCES,DELETE,UPDATE ON TABLE public.genre TO employee;
GRANT SELECT ON TABLE public.genre TO guest;


--
-- TOC entry 3629 (class 0 OID 0)
-- Dependencies: 231
-- Name: TABLE genre_record; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.genre_record TO "user";
GRANT SELECT,INSERT,REFERENCES,DELETE,UPDATE ON TABLE public.genre_record TO employee;
GRANT SELECT ON TABLE public.genre_record TO guest;


--
-- TOC entry 3630 (class 0 OID 0)
-- Dependencies: 243
-- Name: TABLE inventory; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,UPDATE ON TABLE public.inventory TO employee;


--
-- TOC entry 3632 (class 0 OID 0)
-- Dependencies: 252
-- Name: TABLE log; Type: ACL; Schema: public; Owner: postgres
--

GRANT INSERT ON TABLE public.log TO employee;
GRANT INSERT ON TABLE public.log TO guest;
GRANT INSERT ON TABLE public.log TO "user";


--
-- TOC entry 3633 (class 0 OID 0)
-- Dependencies: 237
-- Name: TABLE order_line; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,REFERENCES,DELETE,UPDATE ON TABLE public.order_line TO "user";
GRANT SELECT ON TABLE public.order_line TO employee;


--
-- TOC entry 3634 (class 0 OID 0)
-- Dependencies: 227
-- Name: TABLE product; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.product TO "user";
GRANT SELECT,INSERT,REFERENCES,DELETE,UPDATE ON TABLE public.product TO employee;
GRANT SELECT ON TABLE public.product TO guest;


--
-- TOC entry 3635 (class 0 OID 0)
-- Dependencies: 245
-- Name: TABLE purchase_order; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,UPDATE ON TABLE public.purchase_order TO employee;


--
-- TOC entry 3636 (class 0 OID 0)
-- Dependencies: 247
-- Name: TABLE purchase_order_line; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,UPDATE ON TABLE public.purchase_order_line TO employee;


--
-- TOC entry 3637 (class 0 OID 0)
-- Dependencies: 221
-- Name: TABLE record; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.record TO "user";
GRANT SELECT,INSERT,REFERENCES,DELETE,UPDATE ON TABLE public.record TO employee;
GRANT SELECT ON TABLE public.record TO guest;


--
-- TOC entry 3638 (class 0 OID 0)
-- Dependencies: 239
-- Name: TABLE review; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,REFERENCES,DELETE,UPDATE ON TABLE public.review TO "user";
GRANT SELECT ON TABLE public.review TO employee;
GRANT SELECT ON TABLE public.review TO guest;


--
-- TOC entry 3639 (class 0 OID 0)
-- Dependencies: 241
-- Name: TABLE role; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.role TO "user";
GRANT SELECT ON TABLE public.role TO employee;
GRANT SELECT,REFERENCES ON TABLE public.role TO guest;


--
-- TOC entry 3640 (class 0 OID 0)
-- Dependencies: 236
-- Name: TABLE shop_order; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,REFERENCES,DELETE,UPDATE ON TABLE public.shop_order TO "user";
GRANT SELECT,UPDATE ON TABLE public.shop_order TO employee;


--
-- TOC entry 3641 (class 0 OID 0)
-- Dependencies: 233
-- Name: TABLE shopping_cart; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,REFERENCES,DELETE,UPDATE ON TABLE public.shopping_cart TO "user";
GRANT SELECT ON TABLE public.shopping_cart TO employee;


--
-- TOC entry 3642 (class 0 OID 0)
-- Dependencies: 234
-- Name: TABLE shopping_cart_product; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,REFERENCES,DELETE,UPDATE ON TABLE public.shopping_cart_product TO "user";
GRANT SELECT ON TABLE public.shopping_cart_product TO employee;


--
-- TOC entry 3643 (class 0 OID 0)
-- Dependencies: 249
-- Name: TABLE supplier; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.supplier TO employee;


-- Completed on 2024-05-03 08:50:10 UTC

--
-- PostgreSQL database dump complete
--

