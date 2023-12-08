# Indexe und Performance-Analyse

Arbeite in MongoDB Compass oder VS Code und benutze die sample_mflix-Datenbank.

## Index-Verwendung und ESR-Regel
- Verschaffe Dir einen Überblick über die Indexe auf der movies-Collection
- Prüfe für folgende Query, ob ein Index verwendet wird: 
    - Filter: { title: /a/i, year: 1972 }
    - Sort: { "imdb.rating": -1, title: 1}
- Füge einen Index hinzu, der nach der ESR-Regel aufgebaut ist und die Query unterstützt
- Verifiziere, dass der Index verwendet wird

- Projiziere das Ergebnis nun folgendermaßen: { _id: 0, title: 1, "imdb.rating": 1}
- Was ändert sich an der Ausgabe von Explain? Ist dies vorteilhaft?

- Projiziere das Ergebnis nun folgendermaßen: { _id: 0, title: 1, "imdb.rating": 1, plot: 1}
- Was ändert sich an der Ausgabe von Explain? Ist dies vorteilhaft? 

- Lösche den zuvor erstellten Index und erstelle einen neuen, der die Projektion optimal unterstützt

## Indexe für eigenes Datenmodell
Greife auf das Datenmodell aus dem Kapitel M4C1 Daten-Modellierung zurück. 
- Mit welchen Queries ist zu rechnen?
- Welche Indexe könnten diese unterstützen?