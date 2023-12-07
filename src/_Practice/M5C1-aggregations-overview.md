# Aggregationen in MongoDB Compass

Analysiere die movies-Collection in der sample_mflix-Datenbank:

- Finde heraus, welche verschiedenen Typen von Filmen in der Datenbank enthalten sind und wie viele Filme für jeden Typ erfasst sind
- Suche fünf Filme, in denen Al Pacino mitspielt und lade die Kommentare in ein Array
- Finde alle Filme, in denen Al Pacino zusammen mit Robert De Niro spielt, sortiere diese nach IMDB-Rating absteigend; gib' nur Titel und IMDB-Rating aus. Das IMDB-Rating sollte in einem Feld "rating" ausgegeben werden.
- Finde heraus, wieviele Filme Al Pacino je Genre gedreht hat und in welchem Jahr der letzte Film zu diesem Genre gedreht wurde
- Erstelle eine Top Ten der Schauspieler*innen nach Anzahl der Filme
- Füge mit $setWindowFields die Position zur Top Ten hinzu. Was ist der Unterschied zwischen $rank und $denseRank?