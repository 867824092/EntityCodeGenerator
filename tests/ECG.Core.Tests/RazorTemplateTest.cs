using ECG.Contracts;
using Microsoft.Extensions.DependencyInjection;
using RazorViewTemplateEngine.Core.DependencyInjection;
using RazorViewTemplateEngine.Core.Interface;

namespace ECG.Core.Tests; 

public class RazorTemplateTest {
    private IServiceProvider ServiceProvider;
    private IRazorEngine RazorEngine;
    public RazorTemplateTest() {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddRazorViewTemplateEngine(options => {
            options.PhysicalDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            options.FileWildcard = "*.cshtml";
        }, options => {
            options.AddAssemblyReference(typeof(TableStructure));
        });
        ServiceProvider = serviceCollection.BuildServiceProvider();
        RazorEngine = ServiceProvider.GetRequiredService<IRazorEngine>();
    }

    [Fact]
    public async Task should_compile_entity_success() {
        var model = new TableStructure("ECG.Core.Tests", "Student", 2);
        model.Columns.Add(new ColumnDescription()
        { ColumnName = "Id",
          ColumnType = "int",
          Comment = "主键",
          IsCanNull = false });
        model.Columns.Add(new ColumnDescription()
        { ColumnName = "Name",
          ColumnType = "string",
          Comment = "姓名",
          IsCanNull = false });
        var result = await RazorEngine.CompileGenericAsync("/Template.cshtml", model);
        Assert.NotEmpty(result);
        Assert.Equal(@"using System;
namespace ECG.Core.Tests
{
    public class Student
    {
           public int Id { get; set;}
           public string Name { get; set;}
    }
}", result);
    }
}