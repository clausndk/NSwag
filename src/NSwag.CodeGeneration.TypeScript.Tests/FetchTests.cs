﻿using System.Threading.Tasks;
using Xunit;
using NSwag.Generation.WebApi;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class FetchTests
    {
        public class Foo
        {
            public string Bar { get; set; }
        }

        public class DiscussionController : Controller
        {
            [HttpPost]
            public void AddMessage([FromBody]Foo message)
            {
            }
        }

        public class UrlEncodedRequestConsumingController: Controller
        {
            [HttpPost]
            [Consumes("application/x-www-form-urlencoded")]
            public void AddMessage([FromForm]Foo message, [FromForm]string messageId, [FromForm]System.DateTime date, [FromForm]System.Collections.Generic.List<string> list)
            {
            }
        }

        [Fact]
        public async Task When_export_types_is_true_then_add_export_before_classes()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });

            var document = await generator.GenerateForControllerAsync<DiscussionController>();
            var json = document.ToJson();

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Fetch,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m,
                    ExportTypes = true
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            Assert.Contains("export class DiscussionClient", code);
            Assert.Contains("export interface IDiscussionClient", code);
        }

        [Fact]
        public async Task When_export_types_is_false_then_dont_add_export_before_classes()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });

            var document = await generator.GenerateForControllerAsync<DiscussionController>();
            var json = document.ToJson();

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Fetch,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m,
                    ExportTypes = false
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            Assert.DoesNotContain("export class DiscussionClient", code);
            Assert.DoesNotContain("export interface IDiscussionClient", code);
        }

        [Fact]
        public async Task When_consumes_is_url_encoded_then_construct_url_encoded_request()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });

            var document = await generator.GenerateForControllerAsync<UrlEncodedRequestConsumingController>();
            var json = document.ToJson();

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Fetch,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            Assert.Contains("content_", code);
            Assert.DoesNotContain("FormData", code);
            Assert.Contains("\"Content-Type\": \"application/x-www-form-urlencoded\"", code);
        }

        [Fact]
        public async Task When_abort_signal()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });
            
            var document = await generator.GenerateForControllerAsync<UrlEncodedRequestConsumingController>();
            var json = document.ToJson();

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Fetch,
                UseAbortSignal = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.7m
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            Assert.Contains("signal?: AbortSignal", code);
        }

        [Fact]
        public async Task When_no_abort_signal()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });

            var document = await generator.GenerateForControllerAsync<UrlEncodedRequestConsumingController>();
            var json = document.ToJson();

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Fetch,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            Assert.DoesNotContain("signal?: AbortSignal", code);
            Assert.DoesNotContain("signal", code);
        }

        [Fact]
        public async Task When_headers_included()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });

            var document = await generator.GenerateForControllerAsync<DiscussionController>();

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Fetch,
                AddOptionalHeaders = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.7m
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            //Assert.Contains("headers?: HeadersInit", code);
            Assert.Contains("...headers", code);
        }

        [Fact]
        public async Task When_headers_not_included()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });

            var document = await generator.GenerateForControllerAsync<DiscussionController>();

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Fetch,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.7m
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            Assert.DoesNotContain("headers?: HeadersInit", code);
            Assert.DoesNotContain("...headers", code);
        }

        [Fact]
        public async Task When_includeHttpContext()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });

            var document = await generator.GenerateForControllerAsync<UrlEncodedRequestConsumingController>();

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                IncludeHttpContext = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.7m
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            Assert.Contains("httpContext?: HttpContext", code);
            Assert.Contains("context: httpContext", code);
        }

        [Fact]
        public async Task When_no_includeHttpContext()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });

            var document = await generator.GenerateForControllerAsync<UrlEncodedRequestConsumingController>();
            var json = document.ToJson();

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.7m
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            Assert.DoesNotContain("httpContext?: HttpContext", code);
            Assert.DoesNotContain("context: httpContext", code);
        }
    }
}
