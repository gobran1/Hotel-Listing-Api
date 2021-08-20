using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationapi.Data
{
  public class Country
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public string ShortName { get; set; }

    public virtual IList<Hotel> Hotels { get; set; }
  }
}
