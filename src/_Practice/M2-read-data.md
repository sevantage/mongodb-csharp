# Daten lesen
Die folgenden �bungen setzen auf den Sample-Daten in der sample_mflix- und sample_restaurants-Datenbank auf. 
Es ist sinnvoll, die Abfragen zuerst in MongoDB Compass abzubilden und dann in C#. 

- Erstelle ein neues .NET-Projekt in der Solution.
- F�ge eine Projekt-Referenz zu Samples.Base hinzu, um die Klassen Movie und Restaurant verwenden zu k�nnen.
- Verbinde Dich mit Deinem Cluster

# Daten aus der sample_mflix.movies-Collection lesen

- Finde den Film "Jerry Maguire" 
- Finde alle Filme, in denen "Al Pacino" (oder jemand anderes) mitspielt (Cast)
- Begrenze das Ergebnis auf die letzten 10 Filme
- Finde alle Filme, in denen "Al Pacino" in den 1990er Jahren mitgespielt hat
- Sortiere das Ergebnis nach dem IMDB-Rating absteigend
- Gib lediglich den Titel und das Rating aus
- Finde alle Filme des Genres "Comedy" mit einer deutschen und englischen �bersetzung
- Finde den Film "Jerry Maguire" in verschiedenen Schreibweisen

# Daten aus der sample_restaurants.restaurants-Collection lesen

- Finde alle Restaurants in Brooklyn
- Finde alle Restaurants in Manhattan, die amerikanische K�che anbieten
- Finde alle Restaurants in Manhattan, die amerikanische K�che anbieten und mindestens eine Bewertung besitzen, die 
	- Note (grade) "B" und gleichzeitig 
	- eine Punktebewertung (score) von mindestens 30 besitzt