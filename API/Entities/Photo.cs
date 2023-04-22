using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    /*
     * If we are using an entity class as property in another entity class
     * We do not want direct access to that table through DBContext 
     * We can add the Table attribute. this will create the table and add setup the relationship
     * with other table
     */
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }

        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }

    }
}