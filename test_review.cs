using System;
using System.Text.RegularExpressions;

namespace TestReview
{
    class Program
    {
        static void Main()
        {
            // Test 1: Verify empty string split behavior
            var output = "";
            var lines = output.Replace("\r\n", "\n").Split('\n');
            Console.WriteLine($"Test 1 - Empty string: {lines.Length} lines");
            
            // Test 2: Verify regex timeout creates exception
            try
            {
                var pattern = new Regex("(a+)+b", RegexOptions.None, TimeSpan.FromMilliseconds(100));
                var input = new string('a', 50) + "c";
                var result = pattern.IsMatch(input);
                Console.WriteLine($"Test 2 - Pathological regex matched: {result}");
            }
            catch (RegexMatchTimeoutException)
            {
                Console.WriteLine("Test 2 - RegexMatchTimeoutException thrown as expected");
            }
            
            // Test 3: Verify trailing newline behavior
            var output3 = "line1\nline2\n";
            var lines3 = output3.Replace("\r\n", "\n").Split('\n');
            Console.WriteLine($"Test 3 - Trailing newline: {lines3.Length} lines");
            foreach (var line in lines3)
            {
                Console.WriteLine($"  Line: '{line}'");
            }
        }
    }
}
