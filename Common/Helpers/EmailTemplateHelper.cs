using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class EmailTemplateHelper
    {
        public const string VerificationEmail = @"<div style='font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2'>
          <div style='margin:50px auto;width:70%;padding:20px 0'>
            <div style='border-bottom:1px solid #eee'>
              <a href='' style='font-size:1.4em;color: #00466a;text-decoration:none;font-weight:600'>Your verification code</a>
            </div>
            <p>Thank you for joining Lofibay. Use the following code to complete your Sign Up procedures. The code is valid for 5 minutes.</p>
            <h2 style='background: #00466a;margin: 0 auto;width: max-content;padding: 0 10px;color: #fff;border-radius: 4px;'>{0}</h2>
            <p style='font-size:0.9em;'>Best regards,<br />Lofibay</p>
            <hr style='border:none;border-top:1px solid #eee' />
            <div style='float:right;padding:8px 0;color:#aaa;font-size:0.8em;line-height:1;font-weight:300'>
              <p>Lofibay Inc</p>
              <p>1600 Amphitheatre Parkway</p>
              <p>California</p>
            </div>
          </div>
        </div>";
    }
}
