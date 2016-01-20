using System.Collections.Generic;
using Ethereum.RPC.ABI;
using Ethereum.RPC.ABI.Attributes;
using Xunit;
using System.Linq;
using System.Numerics;

namespace Ethereum.ABI.Tests.DNX
{
    public class FunctionAttributeEncodingTests
    {
        [Function(Name ="test", Sha3Signature = "c6888fa1")]
        public class FunctionIntInput
        {
            [Parameter("int")]
            public int A { get; set; }
        }


        [Fact]
        public virtual void ShouldEncodeInt()
        {
            var input = new FunctionIntInput {A = 69};
            var result = new FunctionCallEncoder().EncodeRequest(input);
            Assert.Equal("0xc6888fa10000000000000000000000000000000000000000000000000000000000000045", result);
        }

        [Function(Name = "test", Sha3Signature = "c6888fa1")]
        [FunctionOutput]
        public class FunctionMultipleInputOutput
        {
            [Parameter("string")]
            public string A { get; set; }

            [Parameter("uint[20]", "b", 2)]
            public List<BigInteger> B { get; set; }

            [Parameter("string", 3)]
            public string C { get; set; }
        }

        [Fact]
        public virtual void ShouldEncodeMultipleTypesIncludingDynamicStringAndIntArray()
        {
            var paramsEncoded =
                "00000000000000000000000000000000000000000000000000000000000002c0000000000000000000000000000000000000000000000000000000000003944700000000000000000000000000000000000000000000000000000000000394480000000000000000000000000000000000000000000000000000000000039449000000000000000000000000000000000000000000000000000000000003944a000000000000000000000000000000000000000000000000000000000003944b000000000000000000000000000000000000000000000000000000000003944c000000000000000000000000000000000000000000000000000000000003944d000000000000000000000000000000000000000000000000000000000003944e000000000000000000000000000000000000000000000000000000000003944f0000000000000000000000000000000000000000000000000000000000039450000000000000000000000000000000000000000000000000000000000003945100000000000000000000000000000000000000000000000000000000000394520000000000000000000000000000000000000000000000000000000000039453000000000000000000000000000000000000000000000000000000000003945400000000000000000000000000000000000000000000000000000000000394550000000000000000000000000000000000000000000000000000000000039456000000000000000000000000000000000000000000000000000000000003945700000000000000000000000000000000000000000000000000000000000394580000000000000000000000000000000000000000000000000000000000039459000000000000000000000000000000000000000000000000000000000003945a0000000000000000000000000000000000000000000000000000000000000300000000000000000000000000000000000000000000000000000000000000000568656c6c6f0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005776f726c64000000000000000000000000000000000000000000000000000000";

            var function = new FunctionMultipleInputOutput();
            function.A = "hello";
            function.C = "world";

            var array = new BigInteger[20];
            for (uint i = 0; i < 20; i++)
            {
                array[i] = i + 234567;
            }

            function.B = array.ToList();

            var result = new FunctionCallEncoder().EncodeRequest(function);

            Assert.Equal("0x" + "c6888fa1" + paramsEncoded, result);
        }



        [Fact]
        public virtual void ShouldDecodeMultipleTypesIncludingDynamicStringAndIntArray()
        {

            var functionCallDecoder = new FunctionCallDecoder();

            var array = new uint[20];
            for (uint i = 0; i < 20; i++)
            {
                array[i] = i + 234567;
            }

            var result = functionCallDecoder.
                DecodeOutput<FunctionMultipleInputOutput>("0x" + "00000000000000000000000000000000000000000000000000000000000002c0000000000000000000000000000000000000000000000000000000000003944700000000000000000000000000000000000000000000000000000000000394480000000000000000000000000000000000000000000000000000000000039449000000000000000000000000000000000000000000000000000000000003944a000000000000000000000000000000000000000000000000000000000003944b000000000000000000000000000000000000000000000000000000000003944c000000000000000000000000000000000000000000000000000000000003944d000000000000000000000000000000000000000000000000000000000003944e000000000000000000000000000000000000000000000000000000000003944f0000000000000000000000000000000000000000000000000000000000039450000000000000000000000000000000000000000000000000000000000003945100000000000000000000000000000000000000000000000000000000000394520000000000000000000000000000000000000000000000000000000000039453000000000000000000000000000000000000000000000000000000000003945400000000000000000000000000000000000000000000000000000000000394550000000000000000000000000000000000000000000000000000000000039456000000000000000000000000000000000000000000000000000000000003945700000000000000000000000000000000000000000000000000000000000394580000000000000000000000000000000000000000000000000000000000039459000000000000000000000000000000000000000000000000000000000003945a0000000000000000000000000000000000000000000000000000000000000300000000000000000000000000000000000000000000000000000000000000000568656c6c6f0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005776f726c64000000000000000000000000000000000000000000000000000000"
                );

            Assert.Equal("hello", result.A);
            Assert.Equal("world", result.C);

        }

    }
}