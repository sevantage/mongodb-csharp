# Class maps

Erstelle ein neues Projekt, installiere den MongoDB C# Driver und verbinde Dich mit Deinem Cluster. 

Nimm folgende Klasse in das Projekt auf: 

```csharp
public class MyDocument 
{
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string Name { get; set; } = string.Empty;

    public Guid MyGuid { get; set; } = Guid.NewGuid();

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public int Count { get; set; }

    public int Total { get; set; }

    public double Avg => Count == 0 ? 0d : (Total / (double)Count);
}
```

Konfiguriere die ClassMap, so dass folgendes Schema erreicht wird (deklarativ/imperativ): 

```JSON
{
    "_id": ObjectId("<Id>"),
    "name": "<Name>", 
    "myGuid": "<MyGuid>", 
    "timestamp": ISODate("<Timestamp>"),
    "count": 4, 
    "total": 10,
    "avg": 2.5
}
```

Speichere ein Dokument, um das Schema zu prüfen.

Bonus: erreiche das Schema auch mit der zweiten Vorgehensweise (imperativ/deklarativ).