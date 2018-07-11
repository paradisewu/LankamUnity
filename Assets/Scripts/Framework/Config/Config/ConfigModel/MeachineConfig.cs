using System;

namespace XHConfig
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class MeachineConfig
    {

        public MeachineItem[] itemField;

        [System.Xml.Serialization.XmlElementAttribute("Content")]
        public MeachineItem[] Items
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MeachineItem
    {

        private string AppIdField;

        private string MachineIdField;

        private string UserNameField;

        private string PasswordField;

        private string LocalField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AppId
        {
            get
            {
                return this.AppIdField;
            }
            set
            {
                this.AppIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MachineId
        {
            get
            {
                return this.MachineIdField;
            }
            set
            {
                this.MachineIdField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string UserName
        {
            get
            {
                return this.UserNameField;
            }
            set
            {
                this.UserNameField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Password
        {
            get
            {
                return this.PasswordField;
            }
            set
            {
                this.PasswordField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Local
        {
            get
            {
                return this.LocalField;
            }
            set
            {
                this.LocalField = value;
            }
        }

    }
}
