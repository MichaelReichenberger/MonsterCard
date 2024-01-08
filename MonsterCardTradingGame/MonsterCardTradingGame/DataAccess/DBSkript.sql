create sequence user_id_seq;

alter sequence user_id_seq owner to myuser;

create table users
(
	user_id integer default nextval('user_id_seq'::regclass) not null
		constraint users_pk
			primary key,
	username varchar(32)
		constraint users_pk2
			unique,
	coins integer,
	level integer,
	password varchar(32) not null,
	image varchar(8),
	bio varchar(256),
	role varchar(16) not null,
	name varchar(64)
);

alter table users owner to myuser;

alter sequence user_id_seq owned by users.user_id;

create table card_stacks
(
	user_id integer not null
		constraint card_stacks_users_user_id_fk
			references users,
	unique_id varchar(38) not null
		constraint unique_id
			unique,
	damage double precision,
	name varchar(50),
	element varchar(32) not null,
	constraint card_stacks_pk
		primary key (user_id, unique_id)
);

alter table card_stacks owner to myuser;

create table decks
(
	user_id integer not null
		constraint decks_users_user_id_fk
			references users,
	unique_id varchar(128) not null
		constraint decks_pk2
			unique,
	constraint decks_pk
		primary key (user_id, unique_id)
);

alter table decks owner to myuser;

create table packages
(
	package_id integer not null,
	unique_id varchar(128) not null
		constraint packages_pk2
			unique,
	element varchar(32),
	name varchar(32),
	damage double precision,
	constraint packages_pk
		primary key (package_id, unique_id)
);

alter table packages owner to myuser;

create table user_stats
(
	user_id integer not null
		constraint user_stats_pk
			primary key
		references users,
	games_played integer,
	wins integer,
	losses integer,
	win_loose_ratio double precision,
	elo integer not null,
	username varchar(32) not null
);

alter table user_stats owner to myuser;

create table tradings
(
	sender_id integer not null,
	offered_card varchar(128) not null,
	minimum_damage integer not null,
	deal_id varchar(128) not null,
	constraint tradings_pk
		primary key (sender_id, deal_id)
);

alter table tradings owner to myuser;

create function update_win_loose_ratio() returns trigger
	language plpgsql
as $$
BEGIN
    -- Überprüfung, ob die Verluste 0 sind, um die Division durch Null zu vermeiden.
    -- Wenn wins und losses beide 1 sind, ist das Verhältnis direkt 1.
    IF NEW.losses = 0 AND NEW.wins = 1 THEN
        NEW.win_loose_ratio := 1; -- Setzen Sie einen Standardwert fest, wenn es keine Verluste gibt und 1 Sieg.
    ELSE
        NEW.win_loose_ratio := NEW.wins::decimal / NULLIF(NEW.losses, 0);
    END IF;
    RETURN NEW;
END;
$$;

alter function update_win_loose_ratio() owner to myuser;

