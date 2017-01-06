﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIGAA.Areas.CONV.Models
{
    public class gatbl_DProgramasAnaliticosTemas
    {
        [Key]
        public int lDProgramaAnaliticoTema_id { get; set; }

        [Display(Name = "Unidad", Description = "Unidad")]
        [Required(ErrorMessage = "Debe ingresar el campo {0}")]
        public int lDProgramaAnalitico_id { get; set; }
        
        [Display(Name = "Tema", Description = "Tema")]
        [Required(ErrorMessage = "Debe ingresar el campo {0}")]
        [StringLength(250)]
        public string sTema_desc { get; set; }

        [Display(Name = "Contenido General", Description = "Contenido General")]
        [Required(ErrorMessage = "Debe ingresar el campo {0}")]        
        [DataType(DataType.MultilineText)]
        public string sContenido_gral { get; set; }

        [HiddenInput(DisplayValue = false)]
        [DefaultValue("1")]
        [StringLength(2)]
        public string iEstado_fl { get; set; }

        [HiddenInput(DisplayValue = false)]
        [DefaultValue("1")]
        [StringLength(2)]
        public string iEliminado_fl { get; set; }

        [HiddenInput(DisplayValue = false)]
        [DefaultValue("")]
        [StringLength(50, ErrorMessage = "El campo {0} debe tener {1} caracteres.", MinimumLength = 8)]
        public string sCreated_by { get; set; }

        [HiddenInput(DisplayValue = false)]
        [Display(Name = "Concurrencia")]
        public int iConcurrencia_id { get; set; }

        public virtual gatbl_DProgramasAnaliticos gatbl_DProgramasAnaliticos { get; set; }
        public virtual ICollection<gatbl_DAnalisisConvalidaciones> gatbl_DAnalisisConvalidaciones { get; set; }
        //public virtual ICollection<gatbl_UnidadesConvalidadas> gatbl_UnidadesConvalidadas { get; set; }
    }
}