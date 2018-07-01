using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPictures.Web.Core
{
    [Table("Album")]
    public class Album
    {
        private ICollection<Picture> _pictures;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Picture> Pictures 
        { 
            get => _pictures ?? (_pictures = new List<Picture>());
            set => _pictures = value;
        }
    }
}