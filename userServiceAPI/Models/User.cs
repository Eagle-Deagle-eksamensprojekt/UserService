namespace UserService.Models
{
    using System;
    using System.Collections.Generic;

    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Globalization;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public partial class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        /// <summary>
        /// A unique identifier for the user (MongoDB ObjectId).
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// The user's physical address.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("address")]
        public string Address { get; set; }

        /// <summary>
        /// The date and time when the user was created.
        /// </summary>
        [JsonPropertyName("createdDate")]
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// The user's email address.
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// The user's first name.
        /// </summary>
        [JsonPropertyName("firstname")]
        public string Firstname { get; set; }

        /// <summary>
        /// Specifies whether the user has administrative privileges.
        /// </summary>
        [JsonPropertyName("isAdmin")]
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Specifies whether the user has seller permissions.
        /// </summary>
        [JsonPropertyName("isSeller")]
        public bool IsSeller { get; set; }

        /// <summary>
        /// The user's last name.
        /// </summary>
        [JsonPropertyName("lastname")]
        public string Lastname { get; set; }

        /// <summary>
        /// The user's hashed password.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; }

        /// <summary>
        /// The user's phone number, 8-15 digits.
        /// </summary>
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary> 
        /// Salting for a password so its much harder to figure out the password in the database
        /// <summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]   
        [JsonPropertyName("salt")]
        public string Salt { get; set; }


    }
}