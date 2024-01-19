using System;
using System.Collections.Generic;

namespace biblio.Models;

public partial class Livre
{
    public int Id { get; set; }

    public string Titre { get; set; } = null!;

    public string Auteur { get; set; } = null!;

    public string Resume { get; set; } = null!;

    public bool? EstEmprunte { get; set; }
    public string CombinedTitreAuteur => $"{Titre} de {Auteur}";


    public virtual ICollection<Emprunt> Emprunts { get; set; } = new List<Emprunt>();
}
