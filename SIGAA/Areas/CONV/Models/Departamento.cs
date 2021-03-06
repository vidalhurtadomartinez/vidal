﻿using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGAA.Areas.CONV.Models
{
    public class Departamento
    {
        [Key]
        [Display(Name = "Codigo", Description = "Codigo", Prompt = "Codigo")]
        [Required(ErrorMessage = "Debe ingresar el campo {0}")]
        [StringLength(2, ErrorMessage = "El campo {0} debe tener {1} caracteres.", MinimumLength = 2)]
        public string sDepartamento_fl { get; set; }

        [Required(ErrorMessage ="Debe ingresar el campo {0}")]
        [Display(Name = "Departamento", Description = "Descripcion", Prompt ="Descripcion")]
        [StringLength(30, ErrorMessage ="El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 4)]        
        public string sDescripcion { get; set; }

        [Display(Name = "Activo")]
        [DefaultValue(true)]
        public bool bActivo { get; set; }

        [Display(Name = "Departamento")]
        public virtual ICollection<gatbl_Universidades> gatbl_Universidades { get; set; }
        public virtual ICollection<gatbl_Postulantes> gatbl_Postulantes { get; set; }
    }
}