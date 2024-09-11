using AzGeneratorFromAttribute;
using Models;

[AzGenerated(typeof(MyObject), GeneratorNotTypeRecognized.ThrowException)]
[AzGenerated(typeof(OtherObject))]
internal static partial class Helper { }
