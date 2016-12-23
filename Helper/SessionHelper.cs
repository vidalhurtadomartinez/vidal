﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Security;

namespace Helper
{
    public class SessionHelper
    {
        public static bool ExistUserInSession()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }
        public static void DestroyUserSession()
        {
            FormsAuthentication.SignOut();
        }
        public static int GetUser()
        {
            int user_id = 0;
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity is FormsIdentity)
            {
                FormsAuthenticationTicket ticket = ((FormsIdentity)HttpContext.Current.User.Identity).Ticket;
                if (ticket != null)
                {
                    user_id = Convert.ToInt32(ticket.UserData);
                }
            }
            return user_id;
        }
        public static void AddUserToSession(string id)
        {
            bool persist = true;
            var cookie = FormsAuthentication.GetAuthCookie("Usuario", persist);

            cookie.Name = FormsAuthentication.FormsCookieName;
            cookie.Expires = DateTime.Now.AddMonths(3);
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            var newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, id);
            cookie.Value = FormsAuthentication.Encrypt(newTicket);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }


        //implement for ME
        public static bool ExisteUsuarioEnSession() {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        public static void CerrarSessionDeUsuario() {
            FormsAuthentication.SignOut();
        }

        public static int TraerUsuarioId() {
            int idUsuario = 0;
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity is FormsIdentity) {
                FormsAuthenticationTicket ticket = ((FormsIdentity)HttpContext.Current.User.Identity).Ticket;
                if (ticket != null) {
                    idUsuario = Convert.ToInt32(ticket.UserData);
                }
            }
            return idUsuario;
        }

        public static void AgregarUsuarioASession(string id){
            bool persistencia = true;
            var cookie = FormsAuthentication.GetAuthCookie("usuario", persistencia);
            cookie.Name = FormsAuthentication.FormsCookieName;
            cookie.Expires = DateTime.Now.AddMonths(3);
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            var newTiket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, id);
            cookie.Value = FormsAuthentication.Encrypt(newTiket);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}