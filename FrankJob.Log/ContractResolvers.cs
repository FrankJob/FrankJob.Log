using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Reflection;

namespace FrankJob.Log
{
    //http://www.newtonsoft.com/json/help/html/ContractResolver.htm
    //adiciona todas as propriedades ao log, mesmo as que estiverem com o atributo ignorado
    public class AllPropertiesContractResolver : DefaultContractResolver
    {
        //TODO: Consertar caso o objeto passado tenha uma propriedade igual as colocadas em errorPropertiesIgnored
        //por causa da exclusividade do objeto HttpContext
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            //força a serializar todos os atributos publicos do objeto
            property.Ignored = false;

            return property;
        }
    }

    //teste para serializar o objeto httpcontext
    public class HttpContextContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            //property.Ignored = false;

            var propriedadesIgnoradas = new string[] {
                "Length",
                "Position",
                "ReadTimeout",
                "WriteTimeout",
                "HttpMethod",
                "LastActivityDate",
                "LastUpdatedDate",
                //nao causaram erro no teste
                "Filter",
                "InputStream",
                //"Session"
            };

            if (propriedadesIgnoradas.Contains(property.PropertyName))
                property.Ignored = true;

            return property;
        }
    }
}
