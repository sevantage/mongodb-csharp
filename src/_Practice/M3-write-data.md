# Daten schreiben
Erzeuge Deine eigene Film-Datenbank (alternativ: Restaurant-Datenbank). Arbeite auf einer neuen Datenbank "mymoviedb".

- F�ge Deinen Lieblingsfilm in eine Collection "mymovies" ein, zun�chst mit der Sprache English und einem Darsteller ein
- F�ge weitere drei Filme mit einem unordered Bulk-Insert ein (ebenfalls nur mit der Sprache Englisch)
- Aktualisiere Deinen Lieblingsfilm und f�ge die Sprache Deutsch im Array ein
- Aktualisiere alle Filme und f�ge die Sprache Deutsch hinzu, egal ob sie bereits existiert
- Begutachte Deinen Lieblingsfilm in MongoDB Compass
- Entferne die Sprache Deutsch von allen Filmen von Deinem Lieblingsfilm
- Aktualisiere alle Filme und f�ge die Sprache Deutsch hinzu, sofern diese noch nicht hinzugef�gt wurde
- F�ge weitere Darsteller zu den Filmen mit einem Bulk-Update hinzu; dabei sollten unterschiedliche Schauspieler zu den einzelnen Filmen hinzugef�gt werden
- Aktualisiere drei Filme innerhalb einer Transaktion, z.B. indem Du weitere Darsteller hinzuf�gst