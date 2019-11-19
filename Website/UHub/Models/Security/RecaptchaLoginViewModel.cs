using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UHub.Models.Security
{
    public class RecaptchaLoginViewModel
    {
        /// <summary>
        /// Server-side path for post request 
        /// </summary>
        public string SubmitPath { get; set; } = null;
        /// <summary>
        /// Text value on submit button
        /// </summary>
        public string SubmitText { get; set; } = "Submit";
        /// <summary>
        /// CSS class for submit button
        /// </summary>
        public string SubmitClass { get; set; } = "";
        /// <summary>
        /// Flag to set submit button behavior
        /// </summary>
        public bool UseSubmitBehavior { get; set; } = true;

        public bool RenderScripts { get; set; } = true;

    }
}