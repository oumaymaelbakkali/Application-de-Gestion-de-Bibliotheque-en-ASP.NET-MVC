using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace biblio.Models;

public partial class Emprunt
{
    public int Id { get; set; }

    public int? LivreId { get; set; }

    public int? AbonnéId { get; set; }
   

    public DateTime DateEmprunt { get; set; }
    

    public DateTime DateRetour { get; set; }

    public virtual Abonné? Abonné { get; set; }

    public virtual Livre? Livre { get; set; }
}
