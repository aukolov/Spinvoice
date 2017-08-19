﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Spinvoice.Application.ServerReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServerReference.IFileParseService")]
    public interface IFileParseService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileParseService/Sum", ReplyAction="http://tempuri.org/IFileParseService/SumResponse")]
        int Sum(int a, int b);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileParseService/Sum", ReplyAction="http://tempuri.org/IFileParseService/SumResponse")]
        System.Threading.Tasks.Task<int> SumAsync(int a, int b);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileParseService/Parse", ReplyAction="http://tempuri.org/IFileParseService/ParseResponse")]
        Spinvoice.Common.Domain.Pdf.PdfModel Parse(string filePath);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileParseService/Parse", ReplyAction="http://tempuri.org/IFileParseService/ParseResponse")]
        System.Threading.Tasks.Task<Spinvoice.Common.Domain.Pdf.PdfModel> ParseAsync(string filePath);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IFileParseServiceChannel : Spinvoice.Application.ServerReference.IFileParseService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class FileParseServiceClient : System.ServiceModel.ClientBase<Spinvoice.Application.ServerReference.IFileParseService>, Spinvoice.Application.ServerReference.IFileParseService {
        
        public FileParseServiceClient() {
        }
        
        public FileParseServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public FileParseServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FileParseServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FileParseServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public int Sum(int a, int b) {
            return base.Channel.Sum(a, b);
        }
        
        public System.Threading.Tasks.Task<int> SumAsync(int a, int b) {
            return base.Channel.SumAsync(a, b);
        }
        
        public Spinvoice.Common.Domain.Pdf.PdfModel Parse(string filePath) {
            return base.Channel.Parse(filePath);
        }
        
        public System.Threading.Tasks.Task<Spinvoice.Common.Domain.Pdf.PdfModel> ParseAsync(string filePath) {
            return base.Channel.ParseAsync(filePath);
        }
    }
}
