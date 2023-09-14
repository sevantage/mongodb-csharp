using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M4.C2
{
    /// <summary>
    /// This sample shows an implementation of the Outlier Pattern. 
    /// Blog post 4 and 8 are the outliers (4 has 1000 additional comments, 8 100).
    /// </summary>
    internal class Outlier : ITestDatabaseSample
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var postsColl = db.GetCollection<Post>("m4c2_outlier_posts");
            await postsColl.DeleteManyAsync(FilterDefinition<Post>.Empty);
            var commentsColl = db.GetCollection<Comment>("m4c2_outlier_comments");
            await commentsColl.DeleteManyAsync(FilterDefinition<Comment>.Empty);

            // Generating test data (outliers are Post 4 & 8)
            var posts = GeneratePosts().ToArray();
            await postsColl.InsertManyAsync(posts, new InsertManyOptions() { IsOrdered = false });
            var outliers = posts.Where(x => x.HasMoreComments);
            var comments = outliers.SelectMany((o, i) => Enumerable.Range(1, i == 0 ? 1000 : 100).Select(x => new Comment { PostId = o.Id, Text = $"Comment {x} on {o.Title}" }));
            await commentsColl.InsertManyAsync(comments, new InsertManyOptions() { IsOrdered = false });

            // Load and print posts
            foreach (var p in posts)
            {
                await PrintPostAsync(postsColl, commentsColl, p.Id);
            }
        }

        private async Task PrintPostAsync(IMongoCollection<Post> postsColl, IMongoCollection<Comment> commentsColl, string postId)
        {
            var post = await (await postsColl.FindAsync(x => x.Id == postId)).FirstOrDefaultAsync();
            var outlierText = post.HasMoreComments ? "an" : "no";
            Console.WriteLine($"Post {post.Id} - {post.Title} has {post.Comments.Count} comments and is {outlierText} outlier.");
            if (post.HasMoreComments)
            {
                var noOfAdditionalComments = commentsColl.CountDocuments(x => x.PostId == postId);
                Console.WriteLine($"There are {noOfAdditionalComments} additional comments stored in a separate collection");
            }
        }

        private static IEnumerable<Post> GeneratePosts()
        {
            return Enumerable.Range(1, 10).Select(x => new Post()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Title = $"Post {x}",
                HasMoreComments = new int[] { 4, 8 }.Contains(x),
                Comments = GenerateCommentsForPost(x),
            });
        }

        private static ICollection<CommentBase> GenerateCommentsForPost(int postNo)
        {
            var numPosts = postNo switch
            {
                2 => 10,
                4 => 10,
                8 => 10,
                int p when p % 2 == 0 => 7,
                int p when p % 2 != 0 => 3,
                _ => throw new InvalidOperationException(),
            };
            return Enumerable.Range(1, numPosts).Select(x => new CommentBase()
            {
                Text = $"Comment {postNo}.{x}",
            }).ToList();
        }

        public class Post
        {
            [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

            public string Title { get; set; } = string.Empty;

            public bool HasMoreComments { get; set; }

            public ICollection<CommentBase> Comments { get; set; } = new List<CommentBase>();
        }

        public class CommentBase
        {
            public string Text { get; set; } = string.Empty;
        }

        public class Comment : CommentBase
        {
            [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

            [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
            public string PostId { get; set; } = ObjectId.GenerateNewId().ToString();
        }
    }
}
