--
-- PostgreSQL database dump
--

-- Dumped from database version 14.4
-- Dumped by pg_dump version 14.4

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
-- Name: if_id_exists(integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.if_id_exists(id integer) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
begin
if exists (select from client where client_id=id) then
return 'true';
else
return 'false';
end if;
end;
$$;


--
-- Name: if_username_taken(character varying); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.if_username_taken(usrname character varying) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
begin
if exists (select from client where username=usrname) then
return 'true';
else
return 'false';
end if;
end;
$$;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: client; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.client (
    client_id integer NOT NULL,
    first_name character varying(30) DEFAULT NULL::character varying,
    last_name character varying(30) DEFAULT NULL::character varying,
    gender character(1) DEFAULT NULL::bpchar,
    age integer,
    balance numeric(18,2) DEFAULT 0,
    username character varying(30)
);


--
-- Name: client_client_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.client_client_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: client_client_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.client_client_id_seq OWNED BY public.client.client_id;


--
-- Name: transfer; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.transfer (
    transfer_id integer NOT NULL,
    sender_id integer,
    recipient_id integer,
    amount numeric(18,2),
    transfer_time timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    sender_username character varying(30),
    recipient_username character varying(30)
);


--
-- Name: transfer_transfer_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.transfer_transfer_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: transfer_transfer_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.transfer_transfer_id_seq OWNED BY public.transfer.transfer_id;


--
-- Name: client client_id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.client ALTER COLUMN client_id SET DEFAULT nextval('public.client_client_id_seq'::regclass);


--
-- Name: transfer transfer_id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.transfer ALTER COLUMN transfer_id SET DEFAULT nextval('public.transfer_transfer_id_seq'::regclass);


--
-- Name: client client_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.client
    ADD CONSTRAINT client_pkey PRIMARY KEY (client_id);


--
-- Name: client; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.client ENABLE ROW LEVEL SECURITY;

--
-- Name: client els_policy; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY els_policy ON public.client USING (((username)::text = CURRENT_USER));


--
-- Name: transfer; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.transfer ENABLE ROW LEVEL SECURITY;

--
-- Name: transfer transfer_filter_policy; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY transfer_filter_policy ON public.transfer FOR SELECT USING (((CURRENT_USER = (sender_username)::text) OR (CURRENT_USER = (recipient_username)::text)));


--
-- PostgreSQL database dump complete
--

