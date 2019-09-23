using System;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace WM.Common.Api.Attributes
{
    public class IdentityBasicAuthenticationAttribute : BasicAuthenticationAttribute
    {
        protected override async Task<IPrincipal> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken)
        {

            var genericPrincipal = await Task.Run(() => AuthenticateAndAuthorize(userName, password), cancellationToken).ConfigureAwait(false);
            return genericPrincipal;
        }

        private GenericPrincipal AuthenticateAndAuthorize(string userName, string password)
        {
            GenericPrincipal genericPrincipal = null;
            DirectoryContext dc = new DirectoryContext(DirectoryContextType.Domain, "WE");
            DomainControllerCollection domainControllers = Domain.GetDomain(dc).DomainControllers;

            foreach (DomainController domainController in domainControllers)
            {
                try
                {
                    //File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log.txt", $"{ DateTime.Now } Trying to connecto DC { domainController.Name } { Environment.NewLine }");
                    genericPrincipal = Authorize(domainController.Name, userName, password);
                    break;
                }
                catch (Exception)
                {
                    //File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "logerror.txt", $"{ DateTime.Now } Trying to connecto DC Failed { domainController.Name } { ex.Message } { Environment.NewLine }");
                    //move next, try new domain controller, else it will return null and 500 come out of the api 
                }
            }

            if (genericPrincipal == null)
            {
                dc = new DirectoryContext(DirectoryContextType.Domain, "CORP");
                domainControllers = Domain.GetDomain(dc).DomainControllers;

                foreach (DomainController domainController in domainControllers)
                {
                    try
                    {
                        //File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log.txt", $"{ DateTime.Now } Trying to connecto DC { domainController.Name } { Environment.NewLine }");
                        genericPrincipal = Authorize(domainController.Name, userName, password);
                        break;
                    }
                    catch (Exception)
                    {
                        //File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "logerror.txt", $"{ DateTime.Now } Trying to connecto DC Failed { domainController.Name } { ex.Message } { Environment.NewLine }");
                        //move next, try new domain controller, else it will return null and 500 come out of the api 
                    }
                }
            }

            return genericPrincipal;
        }

        private GenericPrincipal Authorize(string domain, string user, string password)
        {
            //using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain, "DC=" + domain + ",DC=dirsrv,DC=com"))
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain))
            {
                bool isValid = pc.ValidateCredentials(user, password, ContextOptions.Negotiate);
                if (isValid)
                {
                    var userPrincipal = new UserPrincipal(pc) {SamAccountName = user};
                    var searcher = new PrincipalSearcher(userPrincipal);
                    userPrincipal = searcher.FindOne() as UserPrincipal;

                    if (userPrincipal != null)
                    {
                        var adRoles = userPrincipal.GetGroups();

                        return new GenericPrincipal(new GenericIdentity(user), adRoles.Select(m => m.Name).ToArray());
                    }
                }

                return null;
            }
        }
    }
}