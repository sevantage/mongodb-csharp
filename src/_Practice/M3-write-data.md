# Daten schreiben
Erzeuge Deine eigene Film-Datenbank (alternativ: Restaurant-Datenbank). Arbeite auf einer neuen Datenbank "mymoviedb".

- Füge Deinen Lieblingsfilm in eine Collection "mymovies" ein, zunächst mit der Sprache English und einem Darsteller ein
- Füge weitere drei Filme mit einem unordered Bulk-Insert ein (ebenfalls nur mit der Sprache Englisch)
- Aktualisiere Deinen Lieblingsfilm und füge die Sprache Deutsch im Array ein
- Aktualisiere alle Filme und füge die Sprache Deutsch hinzu, egal ob sie bereits existiert
- Begutachte Deinen Lieblingsfilm in MongoDB Compass
- Entferne die Sprache Deutsch von allen Filmen von Deinem Lieblingsfilm
- Aktualisiere alle Filme und füge die Sprache Deutsch hinzu, sofern diese noch nicht hinzugefügt wurde
- Füge weitere Darsteller zu den Filmen mit einem Bulk-Update hinzu; dabei sollten unterschiedliche Schauspieler zu den einzelnen Filmen hinzugefügt werden
- Aktualisiere drei Filme innerhalb einer Transaktion, z.B. indem Du weitere Darsteller hinzufügst