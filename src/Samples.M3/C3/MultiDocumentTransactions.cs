using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Samples.M3.C3
{
    internal class MultiDocumentTransactions : ITestDatabaseSample
    {
        private readonly ConsoleHelper _consoleHelper;

        public MultiDocumentTransactions(ConsoleHelper consoleHelper)
        {
            _consoleHelper = consoleHelper;
        }

        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<TestDocument>("m3c3_multiDocTransactions");
            await coll.DeleteManyAsync(Builders<TestDocument>.Filter.Empty);

            Console.WriteLine("Inserting and updating 10 documents in a transaction.");
            using (var session = await db.Client.StartSessionAsync())
            {
                session.StartTransaction();
                try
                {
                    await coll.InsertManyAsync(session, Enumerable.Range(0, 10).Select(x => new TestDocument() { Value = x, Name = $"Name {x}" }));

                    await coll.UpdateOneAsync(session, x => x.Value == 7, Builders<TestDocument>.Update.Set(x => x.Name, "New name 7"));

                    await session.CommitTransactionAsync();
                }
                finally
                {
                    if (session.IsInTransaction)
                        await session.AbortTransactionAsync();
                }
            }

            _consoleHelper.Separator();
            Console.WriteLine("Updates on same document in different transactions:");
            using (var session1 = await db.Client.StartSessionAsync())
            {
                session1.StartTransaction();
                try
                {
                    Console.WriteLine("session1: Updating name to \"session1: Name 7\".");
                    await coll.UpdateOneAsync(session1, x => x.Value == 7, Builders<TestDocument>.Update.Set(x => x.Name, "session1: Name 7"));

                    using (var session2 = await db.Client.StartSessionAsync())
                    {
                        session2.StartTransaction();
                        try
                        {
                            Console.WriteLine("session2: Updating name to \"session2: Name 7\".");
                            await coll.UpdateOneAsync(session2, x => x.Value == 7, Builders<TestDocument>.Update.Set(x => x.Name, "session2: Name 7"));
                            await session2.CommitTransactionAsync();
                            Console.WriteLine("session2 committed successfully.");
                        }
                        catch (Exception ex)
                        {
                            _consoleHelper.WriteError(ex, "Error in session2");
                        }
                        finally
                        {
                            if (session2.IsInTransaction)
                            {
                                await session2.AbortTransactionAsync();
                                Console.WriteLine("session2 aborted.");
                            }
                        }
                    }

                    await session1.CommitTransactionAsync();
                    Console.WriteLine("session1 committed successfully.");
                }
                catch (Exception ex)
                {
                    _consoleHelper.WriteError(ex, "Error in session1");
                }
                finally
                {
                    if (session1.IsInTransaction)
                    {
                        await session1.AbortTransactionAsync();
                        Console.WriteLine("session1 aborted.");
                    }
                }

                var doc = await (await coll.FindAsync(x => x.Value == 7)).FirstAsync();
                Console.WriteLine($"Name of document with value {doc.Value} is \"{doc.Name}\".");
            }
        }

        public class TestDocument
        {
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

            public int Value { get; set; }

            public string Name { get; set; } = string.Empty;
        }
    }
}
