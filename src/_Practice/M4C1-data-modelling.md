# Daten-Modellierung
Gruppenarbeit: 
Bilde eine Gruppe mit anderen Teilnehmern; sucht Euch ein Beispiel aus Eurem Arbeitsumfeld und analysiert dieses in Hinblick auf:

- Datenzugriffsverhalten
- Geschätzte Menge der Lese-/Schreibzugriffe
- Welche Daten werden häufig aktualisiert?
- Welche Daten und Kennzahlen werden in Ansichten dargestellt?
- Welche Daten werden gemeinsam geladen?
- Welche Abfragen sind aktuell besonders anstrengend für das RDBMS (falls eines verwendet wird)?
- Welche Referenzen bestehen in welcher Kardinalität (1:1, 1:n, 1:zillions)?
- Welche Daten müssen direkt up-to-date sein, welche können zeitgesteuert aktualisiert werden?

Stellt für 2-3 Use Cases ein grobes Datenmodell auf und begründet Eure Entscheidungen. 
- Wie sehen die Dokumente aus?
- Welche Collections werden verwendet?
- Wie werden Referenzen abgebildet?
- Welche Daten werden redundant gehalten?
- Welche Daten werden beim Abspeichern direkt aktualisiert, welche zeitgesteuert?

----------

Falls Euch kein eigenes Beispiel einfällt, könnt Ihr folgendes verwenden: 
- Der Kontext ist die Rechnungserstellung für Kunden
- Folgende Use Cases werden zumindest unterstützt: 
    - Stammdatenpflege am Kunden
    - Erfassung und Aktualisierung von Rechnungen; die Anpassung der Stammdaten (z.B. der Adresse am Kunden) sollte keine Auswirkung auf bereits erstellte Rechnungen haben. 
    - Dashboard zum Kunden, das die Rechnungssummen je Jahr darstellt; außerdem werden die wichtigsten Daten (Nummer, Datum, Auftragssumme) letzten zehn Rechnungen direkt in einer Tabelle dargesetllt
    - Gesamt-Dashboard, das die Gesamtrechnungssummen je Jahr darstellt; außerdem werden die zehn Kunden mit dem größten Auftragsvolumen im aktuellen Jahr dargestellt
    - Kundensuche, die möglichst über alle Eigenschaften des Kunden geht.
Die Pflege der Stammdaten erfolgt selten, z.B. weil die Adresse des Kunden geändert wird; Rechnungen werden für jeden Kunden auf täglicher Basis erstellt. 
Wichtiger ist jedoch die Anzeige der Dashboards, da diese neben den Sachbearbeitern auch vom Vertrieb und dem Management genutzt wird. Ein Zeitversatz von bis zu einem Tag (Gesamt-Dashboard) bzw. 2 Stunden (Kunden-Dashboard) ist ok, solange der Aktualisierungszeitpunkt ersichtlich ist. 