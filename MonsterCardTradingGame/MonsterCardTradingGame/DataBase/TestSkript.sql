
--Card insert for whole collection of cards in the game
INSERT INTO cards (cardid,name, element, damage) VALUES
(1, 'Goblin', 'Water', 10),
(2, 'Goblin', 'Fire', 10),
(3, 'Goblin', 'Normal', 10),
(4, 'Dragon', 'Water', 20),
(5, 'Dragon', 'Fire', 20),
(6, 'Dragon', 'Normal', 20),
(7, 'Elf', 'Water', 15),
(8, 'Elf', 'Fire', 15),
(9, 'Elf', 'Normal', 15),
(10, 'Knight', 'Water', 18),
(11, 'Knight', 'Fire', 18),
(12, 'Knight', 'Normal', 18),
(13, 'Kraken', 'Water', 16),
(14, 'Kraken', 'Fire', 16),
(15, 'Kraken', 'Normal', 16),
(16, 'Orc', 'Water', 14),
(17, 'Orc', 'Fire', 14),
(18, 'Orc', 'Normal', 14),
(19, 'Wizard', 'Water', 17),
(20, 'Wizard', 'Fire', 17),
(21, 'Wizard', 'Normal', 17),
(22, 'Troll', 'Water', 13),
(23, 'Troll', 'Fire', 13),
(24, 'Troll', 'Normal', 13);

--Test stack insert for a stack with random cards in it
INSERT INTO card_stacks (stackID, cardID) VALUES
(1, 5),
(1, 16),
(1, 3),
(1, 9),
(1, 22),
(1, 2),
(1, 14),
(1, 7),
(1, 18),
(1, 12),
(1, 4),
(1, 20),
(1, 11),
(1, 1),
(1, 23),
(1, 8),
(1, 19),
(1, 21),
(1, 6),
(1, 17);


--Create decks table and fill in testdeck
CREATE TABLE decks (
    deckID int,
    stackid int,
    cardid int,
    PRIMARY KEY (deckID, stackID, cardID),
    FOREIGN KEY (cardID) REFERENCES cards(cardID)
);

INSERT INTO decks (deckID, stackID, cardID) VALUES
(1, 1, 3),
(1, 1, 7),
(1, 1, 12),
(1, 1, 15),
(1, 1, 19);




