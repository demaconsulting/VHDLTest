// Copyright (c) 2023 DEMA Consulting
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace DEMAConsulting.VHDLTest.Tests.Run;

/// <summary>
///     Marks test classes that temporarily replace the process-level <c>PATH</c> environment
///     variable so that xUnit runs them with parallelization disabled.
/// </summary>
/// <remarks>
///     <c>PATH</c> is process-global mutable state read by every test that launches a program by
///     bare name. Collections with parallelization disabled run only after all parallel
///     collections have finished, so a temporary <c>PATH</c> replacement cannot be observed by
///     unrelated tests running concurrently.
/// </remarks>
[CollectionDefinition("ProcessPathTests", DisableParallelization = true)]
public sealed class ProcessPathTestsCollection
{
}
