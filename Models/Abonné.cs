using System;
using System.Collections.Generic;

namespace biblio.Models;

public partial class Abonné
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;

    public string Prénom { get; set; } = null!;


    public string CombinedNomPrenom => $"{Nom} - {Prénom}";

    public virtual ICollection<Emprunt> Emprunts { get; set; } = new List<Emprunt>();
}
