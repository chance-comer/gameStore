using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameStore.Domain.Entities;

namespace GameStore.WebUI.Infrastructure.Binders {
    public class CartModelBinder : IModelBinder {
        
        private const string sessionKey = "cart";

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

            Cart cart = null;

            if (controllerContext.HttpContext.Session[sessionKey] != null) {
                cart = (Cart)controllerContext.HttpContext.Session[sessionKey];
            } 

            if (cart == null) { 
                cart = new Cart();
                if (controllerContext.HttpContext.Session != null) {
                    controllerContext.HttpContext.Session[sessionKey] = cart;
                }
            }

            return cart;
        }
    }
}
