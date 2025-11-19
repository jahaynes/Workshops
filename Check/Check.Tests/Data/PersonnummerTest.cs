using System;
using Check.Data;
using CsCheck;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Check.Tests.Data;

[TestClass]
public class PersonnummerTest
{
    [TestMethod]
    public void Test()
    {
        
        // PersonNummerData.GenString.
        
        PersonNummerData.GenString.Sample(pn => Assert.AreEqual("", pn));
    }
    
    [TestMethod]
    public void Long_Range()
    {
        (from t in Gen.Select(Gen.Long, Gen.Long)
                let start = Math.Min(t.V0, t.V1)
                let finish = Math.Max(t.V0, t.V1)
                from value in Gen.Long[start, finish]
                select (value, start, finish))
            .Sample(i => true);
    }
    
}

class PersonNummerData
{
    public static readonly Gen<string> GenString = Gen.String[Gen.Char.AlphaNumeric, 2, 5];
}