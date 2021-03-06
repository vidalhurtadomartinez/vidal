﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIGAA.ViewModels
{
    public class CambioContrasenaWiewModel
    {
        public CambioContrasenaWiewModel() {
            iUsuario_id = 0;
            usr_login = "";
            ContrasenaActual = "";
            ContrasenaNueva = "";
            ConfirmaContrasenaNueva = "";
        }

        [Required(ErrorMessage = "Debe ingresar valores al campo {0} porque es Obligatorio.")]
        [Display(Name = "Usuario :")]
        public int iUsuario_id { get; set; }

        [Required(ErrorMessage = "Debe ingresar valores al campo {0} porque es Obligatorio.")]
        [Display(Name = "Usr Login :")]
        public string usr_login { set; get; }

        [Required(ErrorMessage = "Debe ingresar valores al campo {0} porque es Obligatorio.")]
        [StringLength(30, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual :")]
        public string ContrasenaActual { get; set; }

        [Required(ErrorMessage = "Debe ingresar valores al campo {0} porque es Obligatorio.")]
        [StringLength(30, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Nueva :")]
        public string ContrasenaNueva { get; set; }

        [Required(ErrorMessage = "Debe ingresar valores al campo {0} porque es Obligatorio.")]
        [StringLength(30, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirma Contraseña Nueva :")]
        public string ConfirmaContrasenaNueva { get; set; }
    }
}