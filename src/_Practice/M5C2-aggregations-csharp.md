# Aggregationen in C#

Analysiere die movies-Collection in der sample_mflix-Datenbank, diesmal mit dem MongoDB C# Driver. Verwende möglichst Linq, aber gerne auch das 
Fluent-Interface oder Pipeline-Definitions, wenn Du auf Limitationen stößt

- Finde heraus, welche verschiedenen Typen von Filmen in der Datenbank enthalten sind und wie viele Filme für jeden Typ erfasst sind
- Suche fünf Filme, in denen Al Pacino mitspielt und lade die Kommentare in ein Array
- Finde alle Filme, in denen Al Pacino zusammen mit Robert De Niro spielt, sortiere diese nach IMDB-Rating absteigend; gib' nur Titel und IMDB-Rating aus. Das IMDB-Rating sollte in einem Feld "rating" ausgegeben werden.
- Erstelle eine Top Ten der Schauspieler*innen nach Anzahl der Filme
