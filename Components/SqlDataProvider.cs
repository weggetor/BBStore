// 
// DotNetNuke® - http://www.dotnetnuke.com 
// Copyright (c) 2002-2009 
// by DotNetNuke Corporation 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software. 
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE. 
// 

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Management;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Services.Exceptions;
using Microsoft.ApplicationBlocks.Data;

namespace Bitboxx.DNNModules.BBStore
{

    /// ----------------------------------------------------------------------------- 
    /// <summary> 
    /// SQL Server implementation of the abstract DataProvider class 
    /// </summary> 
    /// <remarks> 
    /// </remarks> 
    /// <history> 
    /// </history> 
    /// ----------------------------------------------------------------------------- 
    public class SqlDataProvider : DataProvider
    {
        #region "Private Members"
        private const string ProviderType = "data";
        private const string ModuleQualifier = "BBStore_";


        private ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);
        private string _connectionString;
        private string _providerPath;
        private string _objectQualifier;
        private string _databaseOwner;
        private CultureInfo cult = CultureInfo.InvariantCulture;
        #endregion

        #region "Constructors"
        public SqlDataProvider()
        {

            // Read the configuration specific information for this provider 
            Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

            // Read the attributes for this provider 

            //Get Connection string from web.config 
            _connectionString = Config.GetConnectionString();

            if (_connectionString == "")
            {
                // Use connection string specified in provider 
                _connectionString = objProvider.Attributes["connectionString"];
            }

            _providerPath = objProvider.Attributes["providerPath"];

            _objectQualifier = objProvider.Attributes["objectQualifier"];
            if (_objectQualifier != "" & _objectQualifier.EndsWith("_") == false)
            {
                _objectQualifier += "_";
            }

            _databaseOwner = objProvider.Attributes["databaseOwner"];
            if (_databaseOwner != "" & _databaseOwner.EndsWith(".") == false)
            {
                _databaseOwner += ".";
            }

        }
        #endregion

        #region "Properties"
        public string ConnectionString
        {
            get { return _connectionString; }
        }
        public string ProviderPath
        {
            get { return _providerPath; }
        }
        public string ObjectQualifier
        {
            get { return _objectQualifier; }
        }
        public string DatabaseOwner
        {
            get { return _databaseOwner; }
        }
        public string Prefix
        {
            get { return _databaseOwner + _objectQualifier + ModuleQualifier; }
        }
        #endregion

        #region "Private Methods"
        private string GetFullyQualifiedName(string name)
        {
            return DatabaseOwner + ObjectQualifier + ModuleQualifier + name;
        }
        private object GetMyNull(object Field)
        {
            return DotNetNuke.Common.Utilities.Null.GetNull(Field, "NULL");
        }
        private object GetNull(object Field)
        {
            return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value);
        }
        #endregion

        #region "Public Methods"

        // SimpleProduct methods
        public override IDataReader GetSimpleProducts(int PortalId, string Language, string Sort, string Where, int Top)
        {
            string selCmd = "SELECT ";
            if (Top > 0)
                selCmd += "TOP " + Top.ToString();
            selCmd += " SimpleProduct.SimpleProductId, SimpleProduct.SubscriberId,SimpleProduct.SupplierId, " +
                      " SimpleProduct.PortalId,SimpleProduct.Image, SimpleProduct.UnitCost, SimpleProduct.OriginalUnitCost," +
                      " SimpleProduct.HideCost,SimpleProduct.TaxPercent,SimpleProduct.UnitId," +
                      " SimpleProduct.ItemNo,SimpleProduct.CreatedOnDate,SimpleProduct.CreatedByUserId," +
                      " SimpleProduct.LastModifiedOnDate,SimpleProduct.LastModifiedByUserId,SimpleProduct.Disabled,SimpleProduct.NoCart," +
                      " SimpleProduct.Weight,"+
                      " Lang.ShortDescription,Lang.ProductDescription, Lang.Attributes, Lang.Name,";
            if (Sort.ToLower() == "random")
                selCmd += " CAST(1001 * RAND(CHECKSUM(NEWID())) AS INTEGER) AS 'SortNo'";
            else
                selCmd += " 0 AS 'SortNo'";

            selCmd += " FROM " + Prefix + "SimpleProduct SimpleProduct" +
                      " INNER JOIN " + Prefix + "SimpleProductLang Lang ON SimpleProduct.SimpleProductId = Lang.SimpleProductId" +
                      " WHERE SimpleProduct.PortalId = " + PortalId.ToString() +
                      " AND Lang.Language = '" + Language + "'" +
                      (Where != String.Empty ? " AND " + Where : "");

            if (Sort != String.Empty)
            {
                if (Sort.ToLower() == "random")
                    selCmd += " ORDER BY SortNo";
                else
                    selCmd += " ORDER BY " + Sort;
            }

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetSimpleProducts(int PortalId, string Language, string Sort, string Where)
        {
            return GetSimpleProducts(PortalId, Language, Sort, Where, 0);
        }
        public override IDataReader GetSimpleProducts(int PortalId, string Language, string Sort)
        {
            return GetSimpleProducts(PortalId, Language, Sort, "", 0);
        }
        public override IDataReader GetSimpleProducts(int PortalId, string Language)
        {
            return GetSimpleProducts(PortalId, Language, "", "", 0);
        }
        public override IDataReader GetSimpleProducts(int PortalId)
        {
            string selCmd = "SELECT SimpleProduct.SimpleProductId, SimpleProduct.SubscriberId,SimpleProduct.SupplierId, " +
                            " SimpleProduct.PortalId,SimpleProduct.Image, SimpleProduct.UnitCost, SimpleProduct.OriginalUnitCost," +
                            " SimpleProduct.HideCost,SimpleProduct.TaxPercent,SimpleProduct.UnitId," +
                            " SimpleProduct.ItemNo,SimpleProduct.CreatedOnDate,SimpleProduct.CreatedByUserId," +
                            " SimpleProduct.LastModifiedOnDate,SimpleProduct.LastModifiedByUserId,SimpleProduct.Disabled,SimpleProduct.NoCart," +
                            " SimpleProduct.Weight," +
                            " '' as ShortDescription, '' as ProductDescription, '' as Attributes, '' as Name," +
                            " 0 AS 'SortNo'" +
                            " FROM " + Prefix + "SimpleProduct SimpleProduct" +
                            " WHERE SimpleProduct.PortalId = " + PortalId.ToString();

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetSimpleProductByProductId(int PortalId, int ProductId)
        {
            string selCmd = "SELECT SimpleProduct.*, 0 AS 'SortNo'" +
                " FROM " + Prefix + "SimpleProduct SimpleProduct" +
                " WHERE SimpleProduct.PortalId = " + PortalId.ToString() +
                " AND SimpleProduct.SimpleProductId = " + ProductId.ToString();

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetSimpleProductByProductId(int PortalId, int ProductId, string Language)
        {
            string selCmd = "SELECT SimpleProduct.SimpleProductId, SimpleProduct.SubscriberId,SimpleProduct.SupplierId, " +
                " SimpleProduct.PortalId,SimpleProduct.Image, SimpleProduct.UnitCost, SimpleProduct.OriginalUnitCost,"+
                " Simpleproduct.HideCost,SimpleProduct.TaxPercent,SimpleProduct.UnitId," +
                " SimpleProduct.ItemNo,SimpleProduct.CreatedOnDate,SimpleProduct.CreatedByUserId," +
                " SimpleProduct.LastModifiedOnDate,SimpleProduct.LastModifiedByUserId,SimpleProduct.Disabled,SimpleProduct.NoCart," +
                " SimpleProduct.Weight," +
                " Lang.ShortDescription,Lang.ProductDescription, Lang.Attributes, Lang.Name," +
                " 0 AS 'SortNo'" +
                " FROM " + Prefix + "SimpleProduct SimpleProduct" +
                " INNER JOIN " + Prefix + "SimpleProductLang Lang ON SimpleProduct.SimpleProductId = Lang.SimpleProductId" +
                " WHERE SimpleProduct.PortalId = " + PortalId.ToString() +
                " AND SimpleProduct.SimpleProductId = " + ProductId.ToString() +
                " AND Lang.Language = '" + Language + "'";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetSimpleProductByModuleId(int PortalId, int ModuleId)
        {
            string selCmd = "SELECT SimpleProduct.*, 0 AS 'SortNo'" +
                " FROM " + Prefix + "SimpleProduct SimpleProduct" +
                " INNER JOIN " + Prefix + "ModuleProduct ModuleProduct ON SimpleProduct.ProductId = ModuleProduct.ProductId" +
                " WHERE SimpleProduct.PortalId = " + PortalId.ToString() +
                " AND ModuleProduct.ModuleId = " + ModuleId.ToString();

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetSimpleProductByModuleId(int PortalId, int ModuleId, string Language)
        {
            string selCmd = "SELECT SimpleProduct.SimpleProductId, SimpleProduct.SubscriberId, SimpleProduct.SupplierId," +
                " SimpleProduct.PortalId,SimpleProduct.Image, SimpleProduct.UnitCost, SimpleProduct.OriginalUnitCost," +
                " Simpleproduct.HideCost,SimpleProduct.TaxPercent,SimpleProduct.UnitId," +
                " SimpleProduct.ItemNo,SimpleProduct.CreatedOnDate,SimpleProduct.CreatedByUserId," +
                " SimpleProduct.LastModifiedOnDate,SimpleProduct.LastModifiedByUserId,Simpleproduct.Disabled,SimpleProduct.NoCart," +
                " SimpleProduct.Weight," +
                " Lang.ShortDescription,Lang.ProductDescription, Lang.Attributes, Lang.Name," +
                " CAST(1001 * RAND(CHECKSUM(NEWID())) AS INTEGER) AS 'SortNo'" +
                " FROM " + Prefix + "SimpleProduct SimpleProduct" +
                " INNER JOIN " + Prefix + "SimpleProductLang Lang ON SimpleProduct.SimpleProductId = Lang.SimpleProductId" +
                " INNER JOIN " + Prefix + "ModuleProduct ModuleProduct ON SimpleProduct.ProductId = ModuleProduct.ProductId" +
                " WHERE SimpleProduct.PortalId = " + PortalId.ToString() +
                " AND ModuleProduct.ModuleId = " + ModuleId.ToString() +
                " AND Lang.Language = '" + Language + "'";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override int NewSimpleProduct(SimpleProductInfo SimpleProduct)
        {
            string insCmd = "SET NOCOUNT ON INSERT INTO " + GetFullyQualifiedName("SimpleProduct") +
                " (SubscriberId,SupplierId,PortalId,Image,ItemNo,UnitCost,OriginalUnitCost,HideCost,TaxPercent,UnitId,CreatedOnDate," +
                  "CreatedByUserId,LastModifiedOnDate,LastModifiedByUserId,Disabled,NoCart,Weight)" +
                " VALUES " +
                " (@SubscriberId,@SupplierId,@PortalId,@Image,@ItemNo,@UnitCost,@OriginalUnitCost,@HideCost,@TaxPercent,@UnitId,@CreatedOnDate,"+
                  "@CreatedByUserId,@LastModifiedOnDate,@LastModifiedByUserId,@Disabled,@NoCart,@Weight)"+
                " SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("SimpleProductId",SimpleProduct.SimpleProductId),
                new SqlParameter("SubscriberId",SimpleProduct.SubscriberId),
                new SqlParameter("SupplierId",SimpleProduct.SupplierId), 
                new SqlParameter("PortalId",SimpleProduct.PortalId),
                new SqlParameter("Image",SimpleProduct.Image),
                new SqlParameter("ItemNo",SimpleProduct.ItemNo),
                new SqlParameter("UnitCost",SimpleProduct.UnitCost),
                new SqlParameter("OriginalUnitCost",SimpleProduct.OriginalUnitCost),
                new SqlParameter("HideCost",SimpleProduct.HideCost),
                new SqlParameter("TaxPercent",SimpleProduct.TaxPercent),
                new SqlParameter("UnitId",GetNull(SimpleProduct.UnitId)), 
                new SqlParameter("CreatedOnDate",SimpleProduct.CreatedOnDate),
                new SqlParameter("CreatedByUserId",SimpleProduct.CreatedByUserId),
                new SqlParameter("LastModifiedOnDate",SimpleProduct.LastModifiedOnDate),
                new SqlParameter("LastModifiedByUserId",SimpleProduct.LastModifiedByUserId),
                new SqlParameter("Disabled",SimpleProduct.Disabled),
                new SqlParameter("NoCart",SimpleProduct.NoCart),
                new SqlParameter("Weight",SimpleProduct.Weight),
            };

            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateSimpleProduct(SimpleProductInfo SimpleProduct)
        {
            string updCmd = "UPDATE " + GetFullyQualifiedName("SimpleProduct") + " SET " +
                " SubscriberId = @SubscriberId," +
                " SupplierId = @SupplierId," + 
                " PortalId = @PortalId," +
                " Image = @Image," +
                " ItemNo = @ItemNo," +
                " UnitCost = @UnitCost," +
                " OriginalUnitCost = @OriginalUnitCost," +
                " HideCost = @HideCost," +
                " TaxPercent = @TaxPercent," +
                " UnitId = @UnitId," +
                " CreatedOnDate = @CreatedOnDate," +
                " CreatedByUserId = @CreatedByUserId," +
                " LastModifiedOnDate = @LastModifiedOnDate," +
                " LastModifiedByUserId = @LastModifiedByUserId," +
                " Disabled = @Disabled," +
                " NoCart = @NoCart," + 
                " Weight = @Weight" + 
                " WHERE SimpleProductId = @SimpleProductId";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("SimpleProductId",SimpleProduct.SimpleProductId),
                new SqlParameter("SubscriberId",SimpleProduct.SubscriberId),
                new SqlParameter("SupplierId",SimpleProduct.SupplierId), 
                new SqlParameter("PortalId",SimpleProduct.PortalId),
                new SqlParameter("Image",SimpleProduct.Image),
                new SqlParameter("ItemNo",SimpleProduct.ItemNo),
                new SqlParameter("UnitCost",SimpleProduct.UnitCost),
                new SqlParameter("OriginalUnitCost",SimpleProduct.OriginalUnitCost),
                new SqlParameter("HideCost",SimpleProduct.HideCost),
                new SqlParameter("TaxPercent",SimpleProduct.TaxPercent),
                new SqlParameter("UnitId",GetNull(SimpleProduct.UnitId)), 
                new SqlParameter("CreatedOnDate",SimpleProduct.CreatedOnDate),
                new SqlParameter("CreatedByUserId",SimpleProduct.CreatedByUserId),
                new SqlParameter("LastModifiedOnDate",SimpleProduct.LastModifiedOnDate),
                new SqlParameter("LastModifiedByUserId",SimpleProduct.LastModifiedByUserId),
                new SqlParameter("Disabled",SimpleProduct.Disabled),
                new SqlParameter("NoCart",SimpleProduct.NoCart),
                new SqlParameter("Weight",SimpleProduct.Weight),
            };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteSimpleProduct(int SimpleProductId)
        {
            string delCmd = "DELETE FROM " + Prefix + "SimpleProduct " +
                "WHERE SimpleProductId = " + SimpleProductId.ToString();
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);
        }
        public override void DeleteSimpleProducts(int PortalId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("SimpleProduct") +
                " WHERE PortalId = @PortalId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("PortalId",PortalId));
        }

        // ModuleProduct methods
        [Obsolete]
        public override IDataReader GetModuleProduct(int PortalId, int ModuleId)
        {
            string selCmd = "SELECT *" +
                " FROM " + Prefix + "ModuleProduct ModuleProduct" +
                " WHERE ModuleProduct.PortalId = " + PortalId.ToString() +
                " AND ModuleProduct.ModuleId = " + ModuleId.ToString();

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        [Obsolete]
        public override int GetModuleProductId(int PortalId, int ModuleId)
        {
            string selCmd = "SELECT ProductId" +
                " FROM " + Prefix + "ModuleProduct ModuleProduct" +
                " WHERE ModuleProduct.PortalId = " + PortalId.ToString() +
                " AND ModuleProduct.ModuleId = " + ModuleId.ToString();
            object result = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd);
            if (result == DBNull.Value)
                return -9999;
            else if (result == null)
                return -99999;
            else
                return (int)result;
        }
        [Obsolete]
        public override int GetModuleProductModuleId(int PortalId, int ProductId)
        {
            string selCmd = "SELECT ModuleId" +
                " FROM " + Prefix + "ModuleProduct ModuleProduct" +
                " WHERE ModuleProduct.PortalId = " + PortalId.ToString() +
                " AND ModuleProduct.ProductId = " + ProductId.ToString();
            object result = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd);
            if (result == DBNull.Value)
                return -9999;
            else if (result == null)
                return -99999;
            else
                return (int)result;
        }
        [Obsolete]
        public override void SetModuleProductId(int PortalId, int ModuleId, int ProductId)
        {
            string sProductId = (ProductId == -9999 ? "NULL" : ProductId.ToString());
            string selCmd = "SELECT COUNT(*) FROM " + Prefix + "ModuleProduct ModuleProduct" +
                " WHERE PortalId = " + PortalId.ToString() +
                " AND ModuleId = " + ModuleId.ToString();
            int Anz = (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd);
            if (Anz > 0)
            {
                string updCmd = "UPDATE " + Prefix + "ModuleProduct" +
                    " SET ProductId = " + sProductId +
                    " WHERE PortalId = " + PortalId.ToString() +
                    " AND ModuleId = " + ModuleId.ToString();
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd);
            }
            else
            {
                string insCmd = "INSERT INTO " + Prefix + "ModuleProduct" +
                        "(PortalId,ModuleId,ProductId) VALUES (" +
                        PortalId.ToString() + "," +
                        ModuleId.ToString() + "," +
                        sProductId + ")";
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd);
            }
        }
        [Obsolete]
        public override string GetModuleProductTemplate(int PortalId, int ModuleId)
        {
            string retVal = "";
            string selCmd = "SELECT ProductTemplateId, Template" +
                " FROM " + Prefix + "ModuleProduct ModuleProduct" +
                " WHERE PortalId = " + PortalId.ToString() +
                " AND ModuleId = " + ModuleId.ToString();
            DataSet dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, selCmd);
            if (dt.Tables.Count > 0 && dt.Tables[0].Rows.Count > 0)
            {
                DataRow dr = dt.Tables[0].Rows[0];
                int productTemplateId = dr["ProductTemplateId"] != DBNull.Value ? (int)dr["ProductTemplateId"] : -1;

                if (productTemplateId >= 0)
                {
                    // We have a template from the ProductTemplate Table
                    selCmd = "SELECT Template FROM " + Prefix + "ProductTemplate WHERE ProductTemplateId = " + productTemplateId.ToString();
                    retVal = (string)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd);
                }
                else
                    retVal = dr["Template"] != DBNull.Value ? (string)dr["Template"] : "";
            }
            return retVal;

        }
        [Obsolete]
        public override void SetModuleProductTemplate(int PortalId, int ModuleId, int ProductTemplateId, string Template)
        {
            string sProductTemplateId = (ProductTemplateId == -1 ? "NULL" : ProductTemplateId.ToString());
            string sTemplate = (Template == String.Empty ? "NULL" : "'" + Template.Replace('\'', '´') + "'");

            string selCmd = "SELECT COUNT(*) FROM " + Prefix + "ModuleProduct ModuleProduct" +
                " WHERE PortalId = " + PortalId.ToString() +
                " AND ModuleId = " + ModuleId.ToString();
            int Anz = (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd);
            if (Anz > 0)
            {
                string updCmd = "UPDATE " + Prefix + "ModuleProduct SET " +
                    " Template = " + sTemplate + "," +
                    " ProductTemplateId = " + sProductTemplateId +
                    " WHERE PortalId = " + PortalId.ToString() +
                    " AND ModuleId = " + ModuleId.ToString();
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd);
            }
            else
            {
                string insCmd = "INSERT INTO " + Prefix + "ModuleProduct" +
                        "(PortalId,ModuleId,Template,ProductTemplateId) VALUES (" +
                        PortalId.ToString() + "," +
                        ModuleId.ToString() + "," +
                        sTemplate + "," +
                        sProductTemplateId + ")";
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd);
            }
        }
        [Obsolete]
        public override void NewModuleProduct(ModuleProductInfo ModuleProduct)
        {
            string insCmd = "INSERT INTO " + Prefix + "ModuleProduct" +
                " (PortalId,ModuleId,ProductId,ProductTemplateId,Template,IsTaxIncluded) VALUES (" +
                ModuleProduct.PortalId.ToString() + "," +
                ModuleProduct.ModuleId.ToString() + "," +
                (ModuleProduct.ProductId == -1 ? "NULL" : ModuleProduct.ProductId.ToString()) + "," +
                (ModuleProduct.ProductTemplateId == -1 ? "NULL" : ModuleProduct.ProductTemplateId.ToString()) + "," +
                (ModuleProduct.Template == "" ? "NULL" : "N'" + ModuleProduct.Template + "'") + "," +
                ModuleProduct.IsTaxIncluded.ToString() + ")";
            SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd);
        }
        [Obsolete]
        public override void UpdateModuleProduct(ModuleProductInfo ModuleProduct)
        {
            string updCmd = "UPDATE " + Prefix + "ModuleProduct SET" +
                " ProductId = " + (ModuleProduct.ProductId == -1 ? "NULL" : ModuleProduct.ProductId.ToString()) + "," +
                " ProductTemplateId = " + (ModuleProduct.ProductTemplateId == -1 ? "NULL" : ModuleProduct.ProductTemplateId.ToString()) + "," +
                " Template = " + (ModuleProduct.Template == "" ? "NULL" : "N'" + ModuleProduct.Template + "'") + "," +
                " IsTaxIncluded = " + ModuleProduct.IsTaxIncluded.ToString() +
                " WHERE PortalId = " + ModuleProduct.PortalId.ToString() +
                " AND ModuleId = " + ModuleProduct.ModuleId.ToString();
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd);
        }

        public override IDataReader GetModuleProducts(int PortalId)
        {
            string selCmd = "SELECT *" +
                            " FROM " + Prefix + "ModuleProduct ModuleProduct" +
                            " WHERE ModuleProduct.PortalId = " + PortalId.ToString();

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override void DeleteModuleProduct(int portalid,int moduleId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "DELETE FROM " + GetFullyQualifiedName("ModuleProduct") +
                " WHERE ModuleId = " + moduleId.ToString() +
                " AND PortalId = " + portalid.ToString());
        }

        // SimpleProductLang methods
        public override IDataReader GetSimpleProductLangs(int SimpleProductId)
        {
            string selCmd = "SELECT * " +
                " FROM " + Prefix + "SimpleProductLang SimpleProductLang" +
                " WHERE SimpleProductLang.SimpleProductId = " + SimpleProductId.ToString();

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetSimpleProductLangsByPortal(int portalId)
        {
            string selCmd = "SELECT * " +
                " FROM " + GetFullyQualifiedName("SimpleProductLang") +
                " WHERE SimpleProductId IN (SELECT SimpleProductId FROM " + GetFullyQualifiedName("SimpleProduct") + " WHERE PortalId = @PortalId)";

            SqlParameter[] SqlParams = new SqlParameter[] {
               new SqlParameter("PortalId",portalId)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetSimpleProductLang(int SimpleProductId, string Language)
        {
            string selCmd = "SELECT * " +
                " FROM " + Prefix + "SimpleProductLang SimpleProductLang" +
                " WHERE SimpleProductLang.SimpleProductId = " + SimpleProductId.ToString() +
                " AND Language = '" + Language + "'";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override void NewSimpleProductLang(SimpleProductLangInfo SimpleProductLang)
        {
            string insCmd = "INSERT INTO " + Prefix + "SimpleProductLang " +
                            "(SimpleProductId,Language,ShortDescription,ProductDescription,Attributes,Name) VALUES " +
                            "(@SimpleProductId,@Language,@ShortDescription,@ProductDescription,@Attributes,@Name)";

            SqlParameter[] SqlParams = new SqlParameter[] {
               new SqlParameter("Name",SimpleProductLang.Name),
               new SqlParameter("ShortDescription",SimpleProductLang.ShortDescription),
               new SqlParameter("ProductDescription",SimpleProductLang.ProductDescription),
               new SqlParameter("Attributes",SimpleProductLang.Attributes),
               new SqlParameter("SimpleProductId",SimpleProductLang.SimpleProductId),
               new SqlParameter("Language",SimpleProductLang.Language) };

            SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateSimpleProductLang(SimpleProductLangInfo SimpleProductLang)
        {
            string updCmd = "UPDATE " + Prefix + "SimpleProductLang SET " +
                " Name = @Name," +
                " ShortDescription = @ShortDescription," + 
                " ProductDescription = @ProductDescription," + 
                " Attributes = @Attributes," +
                " WHERE SimpleProductId = @SimpleProductId," +
                " AND Language = @Language" ;

            SqlParameter[] SqlParams = new SqlParameter[] {
               new SqlParameter("Name",SimpleProductLang.Name),
               new SqlParameter("ShortDescription",SimpleProductLang.ShortDescription),
               new SqlParameter("ProductDescription",SimpleProductLang.ProductDescription),
               new SqlParameter("Attributes",SimpleProductLang.Attributes),
               new SqlParameter("SimpleProductId",SimpleProductLang.SimpleProductId),
               new SqlParameter("Language",SimpleProductLang.Language) };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteSimpleProductLang(int SimpleProductId, string Language)
        {
            string delCmd = "DELETE FROM " + Prefix + "SimpleProductLang " +
                "WHERE SimpleProductId = " + SimpleProductId.ToString() +
                " AND Language = '" + Language + "'";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);
        }
        public override void DeleteSimpleProductLangs(int SimpleProductId)
        {
            string delCmd = "DELETE FROM " + Prefix + "SimpleProductLang " +
                 "WHERE SimpleProductId = " + SimpleProductId.ToString();
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);
        }

        // Customer methods
        public override IDataReader GetCustomerById(int CustomerId)
        {
            string selCmd = "SELECT * FROM " + Prefix + "Customer Customer" +
                " WHERE CustomerId = " + CustomerId.ToString();
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetCustomersByUserId(int PortalId, int UserId)
        {
            string selCmd = "SELECT * FROM " + Prefix + "Customer Customer" +
                " WHERE PortalId = " + PortalId.ToString() +
                " AND UserId = " + UserId.ToString();
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override int NewCustomer(CustomerInfo Customer)
        {
            string insCmd = "set nocount on INSERT INTO " + Prefix + "Customer (UserId,PortalId,Customername) VALUES (" +
                Customer.UserId.ToString() + "," +
                Customer.PortalId.ToString() + ","+
                "@Customername)" +
                " SELECT CAST(scope_identity() AS INTEGER);";
            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd, new SqlParameter("Customername", Customer.CustomerName));
        }
        public override void UpdateCustomer(CustomerInfo Customer)
        {
            string insCmd = "UPDATE " + Prefix + "Customer SET " +
                            " UserId = " + Customer.UserId.ToString() + "," +
                            " PortalId = " + Customer.PortalId.ToString() + "," +
                            " Customername = @Customername " + 
                            " WHERE CustomerId = " + Customer.CustomerId.ToString(); 
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd, new SqlParameter("Customername",Customer.CustomerName));
        }
        public override int SaveCustomer(CustomerInfo customer)
        {
            bool isNew = true;
            string sqlCmd;
            if (customer.CustomerId > -1)
            {
                sqlCmd = "SELECT count(*) FROM " + GetFullyQualifiedName("Customer") + " WHERE CustomerId = @CustomerId";
                isNew = ((int) SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd,
                                                       new SqlParameter("CustomerId", customer.CustomerId)) == 0);
            }
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("UserId", customer.UserId));
            sqlParams.Add(new SqlParameter("PortalId", customer.PortalId));
            sqlParams.Add(new SqlParameter("CustomerName",customer.CustomerName)); 

            if (isNew)
            {
                sqlCmd = "set nocount on INSERT INTO " + GetFullyQualifiedName("Customer") +
                         " (UserId,PortalId,Customername) VALUES " +
                         " (@UserId,@PortalId,@Customername)" +
                         " SELECT CAST(scope_identity() AS INTEGER);";

                return (int) SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd, sqlParams.ToArray());
            }
            else
            {
                sqlCmd = "UPDATE " + GetFullyQualifiedName("Customer") + " SET " +
                         " UserId = @UserId," +
                         " PortalId = @PortalId," +
                         " Customername = @Customername " +
                         " WHERE CustomerId = @CustomerId";
                sqlParams.Add(new SqlParameter("CustomerId", customer.CustomerId));
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams.ToArray());
                return customer.CustomerId;
            }
        }

        public override void DeleteCustomer(int CustomerId)
        {
            string delCmd = "DELETE FROM " + Prefix + "Customer WHERE CustomerId = " + CustomerId.ToString();
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);
        }

        // CustomerAddress methods
        public override IDataReader GetCustomerAddress(int CustomerAddressId)
        {
            string selCmd = "SELECT * FROM " + Prefix + "CustomerAddress CustomerAddress" +
                " WHERE CustomerAddressId = " + CustomerAddressId.ToString();
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetCustomerAddressesByCart(Guid cartid, string language)
        {
            string sqlCmd = "SELECT cua.*, ISNULL(satl.AddressType,'') as AddressType " +
                            " FROM " + GetFullyQualifiedName("CustomerAddress") + " cua " +
                            " LEFT JOIN " + GetFullyQualifiedName("CartAddress") +
                            " caa ON cua.CustomerAddressId = caa.CustomerAddressId" +
                            " LEFT JOIN " + GetFullyQualifiedName("SubscriberAddressTypeLang") +
                            " satl ON caa.SubscriberAddressTypeId = satl.SubscriberAddressTypeId" +
                            " WHERE caa.cartId = @cartId AND satl.Language=@language";

            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("CartId", cartid), 
                    new SqlParameter("Language", language), 
                };
            return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);

        }

        public override IDataReader GetCustomerMainAddress(int CustomerId)
        {
            string selCmd = "SELECT * FROM " + Prefix + "CustomerAddress CustomerAddress" +
                " WHERE CustomerId = " + CustomerId.ToString() + " AND IsDefault = 1";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetCustomerAdditionalAddresses(int CustomerId)
        {
            string selCmd = "SELECT * FROM " + Prefix + "CustomerAddress CustomerAddress" +
                " WHERE CustomerId = " + CustomerId.ToString() + " AND IsDefault = 0";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetCustomerAddresses(int CustomerId)
        {
            string selCmd = "SELECT * FROM " + Prefix + "CustomerAddress CustomerAddress" +
                " WHERE CustomerId = " + CustomerId.ToString();
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        
        public override int NewCustomerAddress(CustomerAddressInfo customerAddress)
        {
            string sqlCmd = "SET NOCOUNT ON INSERT INTO " + GetFullyQualifiedName("CustomerAddress") +
                            " (PortalID,CustomerID,Company,Prefix,Firstname,Middlename,Lastname,Suffix," +
                            "  Unit,Street,Region,Postalcode,City,Suburb,Country,Countrycode,Telephone,"+
                            "  Cell,Fax,Email,IsDefault)" +
                            " VALUES " +
                            " (@PortalID,@CustomerID,@Company,@Prefix,@Firstname,@Middlename,@Lastname,@Suffix," +
                            "  @Unit,@Street,@Region,@Postalcode,@City,@Suburb,@Country,@Countrycode,@Telephone,@Cell,"+
                            "  @Fax,@Email,@IsDefault)" +
                            " SELECT CAST(scope_identity() AS INTEGER);";
            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("CustomerAddressID", customerAddress.CustomerAddressId),
                    new SqlParameter("PortalID", customerAddress.PortalId),
                    new SqlParameter("CustomerID", customerAddress.CustomerId),
                    new SqlParameter("Company", customerAddress.Company),
                    new SqlParameter("Prefix", customerAddress.Prefix),
                    new SqlParameter("Firstname", customerAddress.Firstname),
                    new SqlParameter("Middlename", customerAddress.Middlename),
                    new SqlParameter("Lastname", customerAddress.Lastname),
                    new SqlParameter("Suffix", customerAddress.Suffix),
                    new SqlParameter("Unit", customerAddress.Unit),
                    new SqlParameter("Street", customerAddress.Street),
                    new SqlParameter("Region", customerAddress.Region),
                    new SqlParameter("Postalcode", customerAddress.PostalCode),
                    new SqlParameter("City", customerAddress.City),
                    new SqlParameter("Suburb", customerAddress.Suburb),
                    new SqlParameter("Country", customerAddress.Country),
                    new SqlParameter("Countrycode", customerAddress.CountryCode),
                    new SqlParameter("Telephone", customerAddress.Telephone),
                    new SqlParameter("Cell", customerAddress.Cell),
                    new SqlParameter("Fax", customerAddress.Fax),
                    new SqlParameter("Email", customerAddress.Email),
                    new SqlParameter("IsDefault", customerAddress.IsDefault)
                };
            
            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd,sqlParams);
        }
        public override void UpdateCustomerAddress(CustomerAddressInfo customerAddress)
        {
            string sqlCmd = "UPDATE " + GetFullyQualifiedName("CustomerAddress") + " SET " +
                            "PortalId = @PortalId," +
                            "CustomerId = @CustomerId," +
                            "Company = @Company," +
                            "Prefix = @Prefix," +
                            "Firstname = @Firstname," +
                            "Middlename = @Middlename," +
                            "Lastname = @Lastname," +
                            "Suffix = @Suffix," +
                            "Unit = @Unit," +
                            "Street = @Street," +
                            "Region = @Region," +
                            "PostalCode = @Postalcode," +
                            "City = @City," +
                            "Suburb = @Suburb," +
                            "Country = @Country," +
                            "CountryCode = @CountryCode," +
                            "Telephone = @Telephone," +
                            "Cell = @Cell," +
                            "Fax = @Fax," +
                            "Email = @Email," +
                            "IsDefault = @IsDefault " +
                            "WHERE CustomerAddressId = @CustomerAddressId";
            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("CustomerAddressID", customerAddress.CustomerAddressId),
                    new SqlParameter("PortalID", customerAddress.PortalId),
                    new SqlParameter("CustomerID", customerAddress.CustomerId),
                    new SqlParameter("Company", customerAddress.Company),
                    new SqlParameter("Prefix", customerAddress.Prefix),
                    new SqlParameter("Firstname", customerAddress.Firstname),
                    new SqlParameter("Middlename", customerAddress.Middlename),
                    new SqlParameter("Lastname", customerAddress.Lastname),
                    new SqlParameter("Suffix", customerAddress.Suffix),
                    new SqlParameter("Unit", customerAddress.Unit),
                    new SqlParameter("Street", customerAddress.Street),
                    new SqlParameter("Region", customerAddress.Region),
                    new SqlParameter("Postalcode", customerAddress.PostalCode),
                    new SqlParameter("City", customerAddress.City),
                    new SqlParameter("Suburb", customerAddress.Suburb),
                    new SqlParameter("Country", customerAddress.Country),
                    new SqlParameter("Countrycode", customerAddress.CountryCode),
                    new SqlParameter("Telephone", customerAddress.Telephone),
                    new SqlParameter("Cell", customerAddress.Cell),
                    new SqlParameter("Fax", customerAddress.Fax),
                    new SqlParameter("Email", customerAddress.Email),
                    new SqlParameter("IsDefault", customerAddress.IsDefault)
                };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override void DeleteCustomerAddress(int CustomerAddressId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("CustomerAddress") + 
                " WHERE CustomerAddressId = " + CustomerAddressId.ToString();
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);

        }

        // Cart methods
        public override IDataReader GetCart(Guid CartId)
        {
            string selCmd = "WITH Product as (SELECT Product.CartProductID," +
                "Product.Quantity," +
                "Product.UnitCost," +
                "Product.TaxPercent," +
                "ISNULL((SELECT SUM(PriceAlteration )" +
                " FROM " + Prefix + "CartProductOption ProductOption " +
                " WHERE ProductOption.CartProductId = Product.CartProductId),0.00)  as PriceAlteration" +
                " FROM " + Prefix + "CartProduct Product WHERE Product.CartId = '" + CartId.ToString() + "')," +
                "Additional as (SELECT Additional.CartAdditionalCostId," +
                "Additional.Quantity," +
                "Additional.UnitCost," +
                "Additional.TaxPercent" +
                " FROM " + Prefix + "CartAdditionalCost Additional WHERE Additional.CartId = '" + CartId.ToString() + "')" +
                "SELECT Cart.*," +
                "ISNULL(( SELECT SUM(Product.Quantity * (Product.UnitCost + Product.PriceAlteration)) FROM Product ),0.00) as OrderTotal," +
                "ISNULL(( SELECT SUM(Product.Quantity * (Product.UnitCost + Product.PriceAlteration) * Product.Taxpercent / 100) FROM Product),0.00) as OrderTax," +
                "ISNULL(( SELECT SUM(Additional.Quantity * Additional.UnitCost) FROM Additional),0.00) as AdditionalTotal," +
                "ISNULL(( SELECT SUM(Additional.Quantity * Additional.UnitCost * Additional.Taxpercent / 100) FROM Additional),0.00) as AdditionalTax " +
                " FROM " + Prefix + "Cart Cart " +
                " WHERE Cart.CartId = '" + CartId.ToString() + "'";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override void NewCart(int portalId, CartInfo cart)
        {
            string sqlCmd = "INSERT INTO " + GetFullyQualifiedName("Cart") +
                            " (CartID,StoreGuid,PortalID,SubscriberID,CustomerID,CustomerPaymentProviderID," +
                            "  Comment,Currency,Total,AttachName,AttachContentType,Attachment,CartName)" +
                            " VALUES " +
                            " (@CartID,@StoreGuid,@PortalID,@SubscriberID,@CustomerID,@CustomerPaymentProviderID," +
                            "  @Comment,@Currency,@Total,@AttachName,@AttachContentType,@Attachment, @CartName)";

            SqlParameter[] SqlParams = new SqlParameter[]
                {
                    new SqlParameter("CartID", cart.CartID),
                    new SqlParameter("StoreGuid", cart.StoreGuid),
                    new SqlParameter("PortalID", portalId),
                    new SqlParameter("SubscriberID", cart.SubscriberID),
                    new SqlParameter("CustomerID", GetNull(cart.CustomerID)),
                    new SqlParameter("CustomerPaymentProviderID", GetNull(cart.CustomerPaymentProviderID)),
                    new SqlParameter("Comment", cart.Comment),
                    new SqlParameter("Currency", cart.Currency),
                    new SqlParameter("Total", cart.Total),
                    new SqlParameter("AttachName", cart.AttachName),
                    new SqlParameter("AttachContentType", cart.AttachContentType),
                    new SqlParameter("Attachment", SqlDbType.VarBinary) {Value = cart.Attachment}, 
                    new SqlParameter("CartName", cart.CartName)
                };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, SqlParams);
        }
        public override void UpdateCart(int PortalId, CartInfo Cart)
        {
            string updCmd = "UPDATE " + Prefix + "Cart SET " +
                "StoreGuid = @StoreGuid," +
                "SubscriberId = " + Cart.SubscriberID.ToString() + "," +
                "CustomerId = " + GetMyNull(Cart.CustomerID).ToString() + "," +
                "CustomerPaymentProviderID = " + GetMyNull(Cart.CustomerPaymentProviderID).ToString() + "," +
                "Comment = @Comment," +
                "Currency = @Currency," +
                "Total = " + Cart.Total.ToString(cult) + "," +
                "AttachName = @AttachName,"+
                "Attachment = @Attachment," +
                "AttachContentType = @AttachContentType," +
                "CartName = @CartName" +
                " WHERE PortalId = " + PortalId.ToString() +
                " AND cartID = '" + Cart.CartID.ToString() + "'";

            SqlParameter paramAttach = new SqlParameter("Attachment", SqlDbType.VarBinary);
            paramAttach.Value = Cart.Attachment;
            if (Cart.Attachment != null)
                paramAttach.Size = Cart.Attachment.Length;

            SqlParameter[] sqlParams = new SqlParameter[]
                                        {
                                            paramAttach,
                                            new SqlParameter("AttachName", Cart.AttachName),
                                            new SqlParameter("AttachContentType", Cart.AttachContentType),
                                            new SqlParameter("CartName", Cart.CartName),
                                            new SqlParameter("Comment", Cart.Comment ?? ""),
                                            new SqlParameter("Currency", Cart.Currency),
                                            new SqlParameter("StoreGuid", Cart.StoreGuid), 
                                        };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, sqlParams);
        }
        public override void DeleteCart(Guid CartId)
        {
            string delCmd = "DELETE FROM " + Prefix + "Cart WHERE CartId = '" + CartId.ToString() + "'";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);
        }
        public override IDataReader GetCartTax(int PortalId, Guid CartId)
        {
            string selCmd = "SELECT TaxPercent, SUM(TaxTotal) AS TaxTotal FROM " +
                "(SELECT TaxPercent, SUM(Quantity * UnitCost * TaxPercent / 100) AS TaxTotal FROM " + Prefix + "CartProduct WHERE CartId = '" + CartId.ToString() + "' GROUP BY TaxPercent " +
                " UNION" +
                " SELECT TaxPercent, SUM(Quantity * UnitCost * TaxPercent / 100) AS TaxTotal FROM " + Prefix + "CartAdditionalCost WHERE CartId = '" + CartId.ToString() + "' GROUP BY TaxPercent ) AS tmptbl " +
                " WHERE TaxPercent > 0" +
                " GROUP BY TaxPercent";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }

        public override void UpdateCartCustomerId(Guid cartId, int customerId)
        {
            string sqlCmd = "SELECT CustomerId FROM " + GetFullyQualifiedName("Cart") + " WHERE CartId = @CartId";

            object oldValue = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("CartId", cartId));
            int oldId = -1;
            if (oldValue != DBNull.Value)
                oldId = (int) oldValue;
            if (oldId != customerId)
            {
                sqlCmd = "DELETE FROM " + GetFullyQualifiedName("CartAdditionalCost") + " WHERE CartId = @CartId";
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("CartId", cartId));
                sqlCmd = "DELETE FROM " + GetFullyQualifiedName("CartAddress") + " WHERE CartId = @CartId";
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("CartId", cartId));
                sqlCmd = "UPDATE " + GetFullyQualifiedName("Cart") + " SET " +
                         " CustomerId = @Customerid, " +
                         " CustomerPaymentProviderID = @CustomerPaymentProviderID" +
                         " WHERE cartID = @Cartid";

                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd,
                                          customerId == -1 ? new SqlParameter("CustomerId", DBNull.Value) : new SqlParameter("CustomerId", customerId),
                                          new SqlParameter("CartId", cartId),
                                          new SqlParameter("CustomerPaymentProviderId", DBNull.Value));
            }
        }
        public override void UpdateCartCustomerPaymentProviderId(Guid cartId, int customerPaymentProviderId)
        {
            string updCmd = "UPDATE " + Prefix + "Cart SET " +
                " CustomerPaymentProviderId = " + (customerPaymentProviderId == -1 ? "Null" : customerPaymentProviderId.ToString()) +
                " WHERE cartID = '" + cartId.ToString() + "'";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd);
        }
        public override string SerializeCart(Guid cartId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("Cart") + " WHERE CartID = @cartId";
            SqlDataReader dr = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("CartID", cartId));
            CartInfo cart = (CartInfo) CBO.FillObject(dr, typeof (CartInfo));
            dr.Close();

            sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("CartProduct") + " WHERE CartID = @cartId";
            dr = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("CartID", cartId));
            List<CartProductInfo> cartProducts = CBO.FillCollection<CartProductInfo>(dr);
            dr.Close();
            foreach (CartProductInfo cartProduct in cartProducts)
            {
                sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("CartProductOption") + " WHERE CartProductID = @cartProductID";
                dr = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("CartProductID", cartProduct.CartProductId));
                List<CartProductOptionInfo> cartProductOptions = CBO.FillCollection<CartProductOptionInfo>(dr);
                dr.Close();
                cartProduct.CartProductOptions = cartProductOptions;
            }
            cart.CartProducts = cartProducts;

            sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("CartAdditionalCost") + " WHERE CartID = @cartId";
            dr = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("CartID", cartId));
            List<CartAdditionalCostInfo> cartAdditionalCosts = CBO.FillCollection<CartAdditionalCostInfo>(dr);
            dr.Close();
            cart.CartAdditionalCosts = cartAdditionalCosts;

            sqlCmd = " SELECT * FROM " + GetFullyQualifiedName("CartAddress") + " WHERE CartId = @cartId";
            dr = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("CartID", cartId));
            List<CartAddressInfo> cartAddresses = CBO.FillCollection<CartAddressInfo>(dr);
            dr.Close();
            foreach (CartAddressInfo cartAddress in cartAddresses)
            {
                sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("CustomerAddress") +
                         " WHERE CustomerAddressId = @CustomerAddressId";
                dr = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("CustomerAddressId", cartAddress.CustomerAddressId));
                CustomerAddressInfo customerAddress = (CustomerAddressInfo) CBO.FillObject(dr, typeof (CustomerAddressInfo));
                dr.Close();
                cartAddress.CustomerAddress = customerAddress;
            }
            cart.CartAddresses = cartAddresses;

            //XmlSerializer serializer = new XmlSerializer(cart.GetType());
            //using (StringWriter writer = new StringWriter())
            //{
            //    serializer.Serialize(writer, cart);
            //    return writer.ToString();
            //}

            XmlSerializer xmlSerializer = new XmlSerializer(cart.GetType());
            MemoryStream stream = new MemoryStream();
            UTF8Encoding enc = new UTF8Encoding();
            XmlTextWriter xmlSink = new XmlTextWriter(stream, enc);
            xmlSerializer.Serialize(xmlSink, cart);
            byte[] utf8EncodedData = stream.ToArray();
            return enc.GetString(utf8EncodedData);
        }
        public override CartInfo DeserializeCart(int portalId, int userId,Guid cartId, string cartXml)
        {
            CartInfo cart = new CartInfo();
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(CartInfo));
                StringReader stringReader = new StringReader(cartXml);
                XmlTextReader xmlReader = new XmlTextReader(stringReader);
                cart = (CartInfo)ser.Deserialize(xmlReader);
                xmlReader.Close();
                stringReader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            
            DeleteCart(cartId);
            cart.CartID = cartId;

            // Den Kunden ggfs. Anlegen
            int customerId = GetImportRelationOwnId(portalId, "CUSTOMER",cart.CustomerID, cart.StoreGuid);
            if (customerId == -1)
            {
                CustomerInfo customer = new CustomerInfo();
                customer.PortalId = portalId;
                customer.UserId = userId;
                if (cart.CartAddresses.Count > 0)
                {
                    customer.CustomerName =
                        (cart.CartAddresses[0].CustomerAddress.Company + " " + cart.CartAddresses[0].CustomerAddress.Firstname + " " +
                         cart.CartAddresses[0].CustomerAddress.Lastname).Trim();
                }
                else
                {
                    customer.CustomerName = UserController.GetCurrentUserInfo().Username;
                }
                customerId = NewCustomer(customer);
                NewImportRelation(portalId, "CUSTOMER", customerId, cart.CustomerID, cart.StoreGuid);
            }
            else
            {
                CustomerInfo customer = (CustomerInfo)CBO.FillObject(DataProvider.Instance().GetCustomerById(customerId), typeof(CustomerInfo));
                if (customer != null)
                {
                    customer.UserId = userId;
                    UpdateCustomer(customer);
                }
            }
            cart.CustomerID = customerId;

            NewCart(portalId, cart);

            // Die Kundenadressen ggfs anlegen
            foreach (CartAddressInfo cartAddress in cart.CartAddresses)
            {
                CustomerAddressInfo customerAddress = cartAddress.CustomerAddress;
                customerAddress.CustomerId = customerId;
                int customerAddressId = GetImportRelationOwnId(portalId, "CUSTOMERADDRESS", customerAddress.CustomerAddressId, cart.StoreGuid);

                if (customerAddressId == -1)
                {
                    customerAddressId = NewCustomerAddress(customerAddress);
                    NewImportRelation(portalId, "CUSTOMERADDRESS", customerAddressId, customerAddress.CustomerAddressId, cart.StoreGuid);
                }
                else
                {
                    customerAddress.CustomerAddressId = customerAddressId;
                    UpdateCustomerAddress(customerAddress);
                }
                cartAddress.CartID = cartId;
                cartAddress.CustomerAddressId = customerAddressId;
                NewCartAddress(cartAddress);
            }

            foreach (CartAdditionalCostInfo additionalCost in cart.CartAdditionalCosts)
            {
                additionalCost.CartId = cartId;
                NewCartAdditionalCost(additionalCost);
            }

            ModuleController objModules = new ModuleController();
            ModuleInfo productModule = objModules.GetModuleByDefinition(portalId, "BBStore Product");
            foreach (CartProductInfo cartProduct in cart.CartProducts)
            {
                int productId = GetImportRelationOwnId(portalId, "PRODUCT", cartProduct.ProductId, cart.StoreGuid);

                if (productId > 0)
                {
                    IDataReader dr = GetSimpleProductByProductId(portalId, productId, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
                    SimpleProductInfo product = (SimpleProductInfo)CBO.FillObject(dr, typeof(SimpleProductInfo));
                    if (product != null)
                    {
                        cartProduct.CartId = cartId;
                        cartProduct.ProductId = productId;
                        cartProduct.ItemNo = product.ItemNo;
                        cartProduct.Image = product.Image;
                        cartProduct.Name = product.Name;
                        cartProduct.Description = product.ProductDescription;
                        int cartProductId = NewCartProduct(cartId, cartProduct);
                        cartProduct.CartProductId = cartProductId;
                        cartProduct.ProductUrl = productModule != null
                                                     ? Globals.NavigateURL(productModule.TabID,"", "productid=" + productId.ToString(),
                                                                           "cpoid=" + cartProductId.ToString())
                                                     : "";
                        UpdateCartProduct(cartProduct);
                        
                        foreach (CartProductOptionInfo cartProductOption in cartProduct.CartProductOptions)
                        {
                            cartProductOption.CartProductId = cartProductId;
                            NewCartProductOption(cartProductId, cartProductOption);
                        }
                    }
                    else
                    {
                        throw new KeyNotFoundException(String.Format("Could not retrieve product with internal ID {0}",productId));
                    }
                }
                else
                {
                    throw new KeyNotFoundException(String.Format("Could not retrieve product with external ID {0}", cartProduct.ProductId));
                }
            }
            return cart;
        }

        // CartAddress methods
        public override int GetCartAddressId(Guid cartid, string kzAddressType)
        {
            string sqlCmd = "SELECT Address.CustomerAddressId FROM " + GetFullyQualifiedName("CartAddress") + " Address " +
                            " INNER JOIN " + GetFullyQualifiedName("SubscriberAddressType") +
                            " AddressType ON Address.SubscriberAddressTypeId = Addresstype.SubscriberAddresstypeid" +
                            " WHERE Addresstype.KzAddressType = @KzAddressType" +
                            " AND Address.CartId = @CartId ";
            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("CartId", cartid), 
                    new SqlParameter("KzAddressType", kzAddressType), 
                };

            object result = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
            if (result != null)
                return (int) result;
            return -1;
        }
        public override IDataReader GetCartAddressByTypeId(Guid cartid, int subscriberAddressTypeId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("CartAddress") +
                            " WHERE CartId = @cartId AND SubscriberAddressTypeId = @SubscriberAddressTypeId";

            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("CartId", cartid), 
                    new SqlParameter("SubscriberAddressTypeId", subscriberAddressTypeId), 
                };
            return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);

        }

        public override IDataReader GetCartAddressesByAddressId(Guid cartid, int customerAddressId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("CartAddress") +
                            " WHERE CartId = @cartId AND CustomerAddressId = @customerAddressId";

            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("CartId", cartid), 
                    new SqlParameter("CustomerAddressId", customerAddressId), 
                };
            return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);

        }
        public override void UpdateCartAddressType(Guid cartId, int portalId, int subscriberId, int customerAddressId, string kzAddressType, bool set)
        {
            string sqlCmd = "SELECT SubscriberAddressTypeId FROM " + GetFullyQualifiedName("SubscriberAddressType") +
                            " WHERE kzAddressType = @kzAddressType" +
                            " AND PortalId = @PortalId" + 
                            " AND SubscriberId = @SubscriberId";

            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("kzAddressType", kzAddressType), 
                    new SqlParameter("PortalId", portalId), 
                    new SqlParameter("SubscriberId", subscriberId), 
                };
            int subscriberAddressTypeId = (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd, sqlParams);

            if (set)
            {
                sqlCmd = "DELETE FROM " + GetFullyQualifiedName("CartAddress") +
                                " WHERE CartId = @cartId AND SubscriberAddressTypeId = @AddressTypeId";

                sqlParams = new SqlParameter[]
                    {
                        new SqlParameter("AddressTypeId", subscriberAddressTypeId), 
                        new SqlParameter("CartId", cartId), 
                    };
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);

                sqlCmd = "INSERT INTO " + GetFullyQualifiedName("CartAddress") +
                         " (CartId,CustomerAddressId,SubscriberAddressTypeId) VALUES" +
                         " (@CartId,@CustomerAddressId,@AddressTypeId)";

                sqlParams = new SqlParameter[]
                    {
                        new SqlParameter("AddressTypeId", subscriberAddressTypeId), 
                        new SqlParameter("CartId", cartId), 
                        new SqlParameter("CustomerAddressId", customerAddressId), 
                    };
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
            }
            else
            {
                sqlCmd = "DELETE FROM " + GetFullyQualifiedName("CartAddress") +
                                " WHERE CartId = @cartId AND SubscriberAddressTypeId = @AddressTypeId AND customerAddressId = @CustomerAddressid";

                sqlParams = new SqlParameter[]
                    {
                        new SqlParameter("AddressTypeId", subscriberAddressTypeId),
                        new SqlParameter("CartId", cartId),
                        new SqlParameter("CustomerAddressId", customerAddressId),
                    };
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
            }
        }
        public override void UpdateCartAddressType(Guid cartId, int portalId, int subscriberId, int customerAddressId, int subscriberAddressTypeId, bool set)
        {
            string sqlCmd;
            SqlParameter[] sqlParams;

            if (set)
            {
                sqlCmd = "DELETE FROM " + GetFullyQualifiedName("CartAddress") +
                                " WHERE CartId = @cartId AND SubscriberAddressTypeId = @AddressTypeId";

                sqlParams = new SqlParameter[]
                    {
                        new SqlParameter("AddressTypeId", subscriberAddressTypeId), 
                        new SqlParameter("CartId", cartId), 
                    };
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
                
                sqlCmd = "INSERT INTO " + GetFullyQualifiedName("CartAddress") +
                         " (CartId,CustomerAddressId,SubscriberAddressTypeId) VALUES" +
                         " (@CartId,@CustomerAddressId,@AddressTypeId)";

                sqlParams = new SqlParameter[]
                    {
                        new SqlParameter("AddressTypeId", subscriberAddressTypeId), 
                        new SqlParameter("CartId", cartId), 
                        new SqlParameter("CustomerAddressId", customerAddressId), 
                    };
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
            }
            else
            {
                sqlCmd = "DELETE FROM " + GetFullyQualifiedName("CartAddress") +
                                " WHERE CartId = @cartId AND SubscriberAddressTypeId = @AddressTypeId AND customerAddressId = @CustomerAddressid";

                sqlParams = new SqlParameter[]
                    {
                        new SqlParameter("AddressTypeId", subscriberAddressTypeId),
                        new SqlParameter("CartId", cartId),
                        new SqlParameter("CustomerAddressId", customerAddressId),
                    };
                    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
            }

        }

        public override void NewCartAddress(CartAddressInfo cartAddress)
        {
            string sqlCmd = "INSERT INTO " + GetFullyQualifiedName("CartAddress") + 
                " (CartId,CustomerAddressId,SubscriberAddressTypeId) VALUES " +
                " (@CartId,@CustomerAddressId,@SubscriberAddressTypeId)";
            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("CartId", cartAddress.CartID), 
                    new SqlParameter("CustomerAddressId",cartAddress.CustomerAddressId), 
                    new SqlParameter("SubscriberAddressTypeId", cartAddress.SubscriberAddressTypeId), 
                };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override bool CheckCartAddresses(Guid cartId, int portalId, int subscriberId)
        {
            string sqlCmd = "SELECT count(*) FROM " + GetFullyQualifiedName("SubscriberAddressType") +
                            " WHERE Mandatory = 1 AND PortalId = @Portalid AND SubscriberId = @SubscriberId AND SubscriberAddressTypeId NOT IN " +
                            " (SELECT SubscriberAddressTypeId FROM " + GetFullyQualifiedName("CartAddress") +
                            "  WHERE CartId = @cartid )";
            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("CartId", cartId), 
                    new SqlParameter("PortalId",portalId), 
                    new SqlParameter("SubscriberId", subscriberId), 
                };

            return ((int) SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd, sqlParams) == 0);
        }
        
        // CartAdditionalCost methods
        public override IDataReader GetCartAdditionalCost(int CartAdditionalCostId)
        {
            string selCmd = "SELECT *," +
                " Quantity * UnitCost As NetTotal," +
                " Quantity * UnitCost * TaxPercent / 100 AS TaxTotal," +
                " Quantity * UnitCost * (100 + TaxPercent) / 100 AS SubTotal" +
                " FROM " + Prefix + "CartAdditionalCost CartAdditionalCost" +
                " WHERE CartAdditionalCostId = " + CartAdditionalCostId.ToString();
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetCartAdditionalCosts(Guid CartId)
        {
            string selCmd = "SELECT *," +
                " Quantity * UnitCost As NetTotal," +
                " Quantity * UnitCost * TaxPercent / 100 AS TaxTotal," +
                " Quantity * UnitCost * (100 + TaxPercent) / 100 AS SubTotal" +
                " FROM " + Prefix + "CartAdditionalCost CartAdditionalCost" +
                " WHERE CartId = '" + CartId.ToString() + "'";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override int NewCartAdditionalCost(CartAdditionalCostInfo CartAdditionalCost)
        {
            string name = CartAdditionalCost.Name;
            if (name.Length > 60)
                name = name.Substring(0, 60);


            string sqlCmd = "set nocount on INSERT INTO " + GetFullyQualifiedName("CartAdditionalCost") +
                 " (CartID,Quantity,Name,Description,Area,UnitCost,TaxPercent) VALUES " +
                 " (@CartID,@Quantity,@Name,@Description,@Area,@UnitCost,@TaxPercent)" +
                 " SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("CartId",CartAdditionalCost.CartId), 
                                               new SqlParameter("Quantity",CartAdditionalCost.Quantity), 
                                               new SqlParameter("Name", CartAdditionalCost.Name), 
                                               new SqlParameter("Description", CartAdditionalCost.Description), 
                                               new SqlParameter("Area", CartAdditionalCost.Area), 
                                               new SqlParameter("UnitCost", CartAdditionalCost.UnitCost), 
                                               new SqlParameter("TaxPercent", CartAdditionalCost.TaxPercent), 
                                           };

            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd,sqlParams);
        }
        public override void UpdateCartAdditionalCost(CartAdditionalCostInfo CartAdditionalCost)
        {
            string name = CartAdditionalCost.Name;
            if (name.Length > 60)
                name = name.Substring(0, 60);

            string sqlCmd = "UPDATE " + GetFullyQualifiedName("CartAdditionalCost") + " SET" +
                            " Quantity = @Quantity," +
                            " Description = @Description," +
                            " Name = @Name," +
                            " Area = @Area," +
                            " UnitCost = @UnitCost," +
                            " TaxPercent = @TaxPercent," +
                            " WHERE CartAdditionalCostId = @CartAdditionalCostId";

            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("Quantity",CartAdditionalCost.Quantity), 
                                               new SqlParameter("Name", CartAdditionalCost.Name), 
                                               new SqlParameter("Description", CartAdditionalCost.Description), 
                                               new SqlParameter("Area", CartAdditionalCost.Area), 
                                               new SqlParameter("UnitCost", CartAdditionalCost.UnitCost), 
                                               new SqlParameter("TaxPercent", CartAdditionalCost.TaxPercent), 
                                               new SqlParameter("CartAdditionalCostId",CartAdditionalCost.CartAdditionalCostId), 
                                           };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override void DeleteCartAdditionalCost(int CartAdditionalCostId)
        {
            string delCmd = "DELETE FROM " + Prefix + "CartAdditionalCost " +
                "WHERE cartAdditionalCostId = " + CartAdditionalCostId.ToString();
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);
        }
        public override void DeleteCartAdditionalCost(Guid CartId, string Area)
        {
            string delCmd = "DELETE FROM " + Prefix + "CartAdditionalCost " +
                " WHERE CartId = '" + CartId.ToString() + "'" +
                " AND Area = '" + Area + "'";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);
        }

        // CartProduct methods
        public override IDataReader GetCartProduct(int CartProductId)
        {
            string selCmd = "SELECT CartProduct.CartProductId, CartProduct.CartID,CartProduct.ProductID,CartProduct.Image, CartProduct.ItemNo," +
                 " CartProduct.Name, CartProduct.Quantity,CartProduct.TaxPercent, CartProduct.Unit, CartProduct.Decimals,"+
                 " CartProduct.Description,CartProduct.ProductUrl,CartProduct.ProductDiscount," +
                 " CartProduct.Weight,CartProduct.ShippingModelId," +
                 " CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0) AS UnitCost," +
                 " CartProduct.Quantity * ROUND((CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)) * (100 + CartProduct.TaxPercent) / 100,2) AS SubTotal," +
                 " ROUND(CartProduct.Quantity * (CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)),2) As NetTotal," +
                 " CartProduct.Quantity * ROUND((CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)) * (100 + CartProduct.TaxPercent) / 100,2) - " +
                 " ROUND(CartProduct.Quantity * (CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)),2) AS TaxTotal" +
                 " FROM " + Prefix + "CartProduct CartProduct" +
                 " LEFT JOIN " + Prefix + "CartProductOption ProductOption ON ProductOption.CartProductID = CartProduct.CartProductID" +
                 " WHERE CartProduct.CartProductId = " + CartProductId.ToString() +
                 " GROUP BY CartProduct.CartProductId, CartProduct.CartID,CartProduct.ProductID,CartProduct.Image, CartProduct.ItemNo," +
                 " CartProduct.Name, CartProduct.Quantity,CartProduct.TaxPercent,CartProduct.Unit, CartProduct.Decimals,"+
                 " CartProduct.UnitCost, CartProduct.Description,CartProduct.ProductUrl,CartProduct.ProductDiscount,CartProduct.Weight,CartProduct.ShippingModelId";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetCartProductByProductId(Guid CartId, int ProductId)
        {
            string selCmd = "SELECT CartProduct.CartProductId, CartProduct.CartID,CartProduct.ProductID,CartProduct.Image, CartProduct.ItemNo," +
                 " CartProduct.Name, CartProduct.Quantity,CartProduct.TaxPercent, CartProduct.Unit, CartProduct.Decimals,"+
                 " CartProduct.Description,CartProduct.ProductUrl,CartProduct.ProductDiscount," +
                 " CartProduct.Weight,CartProduct.ShippingModelId," +
                 " CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0) AS UnitCost," +
                 " CartProduct.Quantity * ROUND((CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)) * (100 + CartProduct.TaxPercent) / 100,2) AS SubTotal," +
                 " ROUND(CartProduct.Quantity * (CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)),2) As NetTotal," +
                 " CartProduct.Quantity * ROUND((CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)) * (100 + CartProduct.TaxPercent) / 100,2) - " +
                 " ROUND(CartProduct.Quantity * (CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)),2) AS TaxTotal" +
                 " FROM " + Prefix + "CartProduct CartProduct" +
                 " LEFT JOIN " + Prefix + "CartProductOption ProductOption ON ProductOption.CartProductID = CartProduct.CartProductID" +
                 " WHERE CartProduct.CartId = '" + CartId.ToString() + "'" +
                 " AND CartProduct.ProductId = " + ProductId.ToString() +
                 " GROUP BY CartProduct.CartProductId, CartProduct.CartID,CartProduct.ProductID,CartProduct.Image, CartProduct.ItemNo," +
                 " CartProduct.Name, CartProduct.Quantity,CartProduct.TaxPercent,CartProduct.Unit, CartProduct.Decimals,"+
                 " CartProduct.UnitCost, CartProduct.Description,CartProduct.ProductUrl,CartProduct.ProductDiscount,CartProduct.Weight,CartProduct.ShippingModelId";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetCartProductByProductIdAndSelectedOptions(Guid CartId, int ProductId, System.Collections.Generic.List<OptionListInfo> SelectedOptions)
        {
            //string optionCriteria = "";
            //if (SelectedOptions != null)
            //{
            //    foreach (OptionListInfo item in SelectedOptions)
            //    {
            //        string OptionName = item.OptionName;
            //        string OptionValue = item.OptionValue;
            //        optionCriteria += " AND CartProduct.CartProductId IN " +
            //            "(SELECT CartProductId FROM " + Prefix + "CartProductOption" +
            //            " WHERE OptionName = '" + item.OptionName + "'" +
            //            " AND OptionValue = '" + item.OptionValue + "')";
            //    }
            //}

            string optionCriteria = " AND CartProduct.CartProductId IN " +
                                    "(SELECT CartProductId FROM " + Prefix + "CartProductOption";
            string optionWhere = "";
            if (SelectedOptions != null && SelectedOptions.Count > 0)
            {
                foreach (OptionListInfo item in SelectedOptions)
                {
                    optionWhere += "(OptionName = '" + item.OptionName + "'" +
                        " AND OptionValue = '" + item.OptionValue + "') OR";
                }
                if (optionWhere.EndsWith("OR") )
                    optionWhere = optionWhere.Substring(0, optionWhere.Length - 2).Trim();
                if (optionWhere != String.Empty)
                    optionCriteria = optionCriteria + " WHERE (" + optionWhere + ") GROUP BY CartProductId HAVING COUNT(cartProductId) = " + SelectedOptions.Count.ToString() + ") ";
                else
                    optionCriteria = optionCriteria + ") ";
            }
            else
            {
                optionCriteria = "";
            }

            string selCmd = "SELECT CartProduct.CartProductId, CartProduct.CartID,CartProduct.ProductID,CartProduct.Image, CartProduct.ItemNo," +
                 " CartProduct.Name, CartProduct.Quantity,CartProduct.TaxPercent, CartProduct.Unit, CartProduct.Decimals,"+
                 " CartProduct.Description,CartProduct.ProductUrl,CartProduct.ProductDiscount," +
                 " CartProduct.Weight,CartProduct.ShippingModelId," +
                 " CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0) AS UnitCost," +
                 " CartProduct.Quantity * ROUND((CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)) * (100 + CartProduct.TaxPercent) / 100,2) AS SubTotal," +
                 " ROUND(CartProduct.Quantity * (CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)),2) As NetTotal," +
                 " CartProduct.Quantity * ROUND((CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)) * (100 + CartProduct.TaxPercent) / 100,2) - " +
                 " ROUND(CartProduct.Quantity * (CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)),2) AS TaxTotal" +
                 " FROM " + Prefix + "CartProduct CartProduct" +
                 " LEFT JOIN " + Prefix + "CartProductOption ProductOption ON ProductOption.CartProductID = CartProduct.CartProductID" +
                 " WHERE CartProduct.CartId = '" + CartId.ToString() + "'" +
                 " AND CartProduct.ProductId = " + ProductId.ToString() +
                 optionCriteria +
                 " GROUP BY CartProduct.CartProductId, CartProduct.CartID,CartProduct.ProductID,CartProduct.Image, CartProduct.ItemNo," +
                 " CartProduct.Name, CartProduct.Quantity,CartProduct.TaxPercent,CartProduct.Unit, CartProduct.Decimals,"+
                 " CartProduct.UnitCost, CartProduct.Description,CartProduct.ProductUrl,CartProduct.ProductDiscount,CartProduct.Weight,CartProduct.ShippingModelId";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetCartProducts(Guid CartId)
        {
            string selCmd = "SELECT CartProduct.CartProductId, CartProduct.CartID,CartProduct.ProductID,CartProduct.Image, CartProduct.ItemNo," +
                " CartProduct.Name, CartProduct.Quantity,CartProduct.TaxPercent, CartProduct.Unit, CartProduct.Decimals,"+
                " CartProduct.Description,CartProduct.ProductUrl,CartProduct.ProductDiscount," +
                " CartProduct.Weight,CartProduct.ShippingModelId," +
                " CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0) AS UnitCost," +
                " CartProduct.Quantity * ROUND((CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)) * (100 + CartProduct.TaxPercent) / 100,2) AS SubTotal," +
                " ROUND(CartProduct.Quantity * (CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)),2) As NetTotal," +
                " CartProduct.Quantity * ROUND((CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)) * (100 + CartProduct.TaxPercent) / 100,2) - " +
                " ROUND(CartProduct.Quantity * (CartProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)),2) AS TaxTotal" +
                " FROM " + Prefix + "CartProduct CartProduct" +
                " LEFT JOIN " + Prefix + "CartProductOption ProductOption ON ProductOption.CartProductID = CartProduct.CartProductID" +
                " WHERE CartProduct.CartID = '" + CartId.ToString() + "'" +
                " GROUP BY CartProduct.CartProductId, CartProduct.CartID,CartProduct.ProductID,CartProduct.Image, CartProduct.ItemNo," +
                " CartProduct.Name, CartProduct.Quantity,CartProduct.TaxPercent,CartProduct.Unit, CartProduct.Decimals,"+
                " CartProduct.UnitCost, CartProduct.Description,CartProduct.ProductUrl,CartProduct.ProductDiscount,CartProduct.Weight,CartProduct.ShippingModelId" +
                " ORDER BY CartProduct.CartProductId";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override int NewCartProduct(Guid CartId, CartProductInfo CartProduct)
        {
            string insCmd = "set nocount on INSERT INTO " + Prefix + "CartProduct " +
                            "(CartID,ProductId,Image,ItemNo,Quantity,Name,Description,ProductUrl,UnitCost,TaxPercent,Unit,Decimals,ProductDiscount,Weight,ShippingModelId) VALUES " +
                            "(@CartID,@ProductId,@Image,@ItemNo,@Quantity,@Name,@Description,@ProductUrl,@UnitCost,@TaxPercent,@Unit,@Decimals,@ProductDiscount,@Weight,@ShippingModelId)" +
                            " SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter[] param = new SqlParameter[] 
            {
                new SqlParameter("CartID",CartProduct.CartId),
                new SqlParameter("ProductId",CartProduct.ProductId),
                new SqlParameter("Image",CartProduct.Image),
                new SqlParameter("ItemNo",CartProduct.ItemNo),
                new SqlParameter("Quantity",CartProduct.Quantity),
                new SqlParameter("Name",CartProduct.Name),
                new SqlParameter("Description",CartProduct.Description),
                new SqlParameter("ProductUrl",CartProduct.ProductUrl),
                new SqlParameter("UnitCost",CartProduct.UnitCost),
                new SqlParameter("TaxPercent",CartProduct.TaxPercent),
                new SqlParameter("Unit",CartProduct.Unit), 
                new SqlParameter("Decimals", CartProduct.Decimals), 
                new SqlParameter("ProductDiscount",CartProduct.ProductDiscount),
                new SqlParameter("Weight",CartProduct.Weight),
                new SqlParameter("ShippingModelId",GetNull(CartProduct.ShippingModelId))
            };

            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd, param);
        }
        public override void UpdateCartProduct(CartProductInfo CartProduct)
        {
            string updCmd = "UPDATE " + Prefix + "CartProduct SET " +
                            " ProductId = @ProductId," +
                            " Image = @Image," +
                            " ItemNo = @ItemNo," +
                            " Quantity = @Quantity," +
                            " Name = @Name," +
                            " Description = @Description," +
                            " ProductUrl = @ProductUrl," +
                            " UnitCost = @UnitCost," +
                            " TaxPercent = @TaxPercent, " +
                            " Unit = @Unit," +
                            " Decimals = @Decimals," + 
                            " ProductDiscount = @ProductDiscount, " +
                            " Weight = @Weight, "+
                            " ShippingModelId = @ShippingModelId " +
                            " WHERE CartProductId = @CartProductId";

            SqlParameter[] param = new SqlParameter[] 
            {
                new SqlParameter("CartProductId",CartProduct.CartProductId),
                new SqlParameter("ProductId",CartProduct.ProductId),
                new SqlParameter("Image",CartProduct.Image),
                new SqlParameter("ItemNo",CartProduct.ItemNo),
                new SqlParameter("Quantity",CartProduct.Quantity),
                new SqlParameter("Name",CartProduct.Name),
                new SqlParameter("Description",CartProduct.Description),
                new SqlParameter("ProductUrl",CartProduct.ProductUrl),
                new SqlParameter("UnitCost",CartProduct.UnitCost),
                new SqlParameter("TaxPercent",CartProduct.TaxPercent),
                new SqlParameter("Unit",CartProduct.Unit), 
                new SqlParameter("Decimals", CartProduct.Decimals), 
                new SqlParameter("ProductDiscount",CartProduct.ProductDiscount),
                new SqlParameter("Weight",CartProduct.Weight),
                new SqlParameter("ShippingModelId",GetNull(CartProduct.ShippingModelId))
            };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, param);
        }
        public override void UpdateCartProductQuantity(int cartProductId, decimal quantity)
        {
            string sqlCmd = "UPDATE " + GetFullyQualifiedName("CartProduct") + " SET " +
                " Quantity = @Quantity" + 
                " WHERE CartProductId = @CartProductId" ;
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("Quantity", quantity), 
                                               new SqlParameter("CartProductId", cartProductId), 
                                           };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override void DeleteCartProduct(int CartProductId)
        {
            string delCmd = "DELETE FROM " + Prefix + "CartProduct " +
                "WHERE CartProductId = " + CartProductId.ToString();
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);
        }

        // CartProductOption methods
        public override IDataReader GetCartProductOption(int CartProductOptionId)
        {
            string selCmd = "SELECT * FROM " + Prefix + "CartProductOption CartProductOption WHERE CartProductOptionId = " + CartProductOptionId.ToString();
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetCartProductOptions(int CartProductId)
        {
            string selCmd = "SELECT * FROM " + Prefix + "CartProductOption CartProductOption WHERE CartProductId = " + CartProductId.ToString();
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override int NewCartProductOption(int CartProductId, CartProductOptionInfo CartProductOption)
        {
            string insCmd = "set nocount on INSERT INTO " + Prefix + "CartProductOption " +
                            "(CartProductID,OptionId,OptionName,OptionValue,OptionImage,OptionDescription,PriceAlteration) VALUES " +
                            "(@CartProductID,@OptionId,@OptionName,@OptionValue,@OptionImage,@OptionDescription,@PriceAlteration) " +
                            "SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter paramImage = new SqlParameter("OptionImage", SqlDbType.VarBinary);
            paramImage.Value = CartProductOption.OptionImage;
            if (CartProductOption.OptionImage != null)
                paramImage.Size = CartProductOption.OptionImage.Length;

            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    paramImage,
                    new SqlParameter("CartProductId",CartProductOption.CartProductId), 
                    new SqlParameter("OptionId", CartProductOption.OptionId), 
                    new SqlParameter("OptionName", CartProductOption.OptionName), 
                    new SqlParameter("OptionValue", CartProductOption.OptionValue), 
                    new SqlParameter("OptionDescription", CartProductOption.OptionDescription), 
                    new SqlParameter("PriceAlteration", CartProductOption.PriceAlteration), 
                };

            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd, sqlParams);
        }
        public override void UpdateCartProductOption(CartProductOptionInfo cartProductOption)
        {
            string updCmd = "UPDATE " + Prefix + "CartProductOption SET " +
                            "CartProductId = @CartProductId," +
                            "OptionId = @OptionId," +
                            "OptionName = @OptionName," +
                            "OptionValue = @OptionValue," +
                            "OptionImage = @OptionImage," +
                            "OptionDescription = @OptionDescription," +
                            "PriceAlteration = @PriceAlteration " +
                            "WHERE CartProductOptionId = @CartProductOptionId";

            SqlParameter paramImage = new SqlParameter("OptionImage", SqlDbType.VarBinary);
            paramImage.Value = cartProductOption.OptionImage;
            if (cartProductOption.OptionImage != null)
                paramImage.Size = cartProductOption.OptionImage.Length;

            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    paramImage,
                    new SqlParameter("CartProductOptionId",cartProductOption.CartProductOptionId), 
                    new SqlParameter("CartProductId",cartProductOption.CartProductId), 
                    new SqlParameter("OptionId", cartProductOption.OptionId), 
                    new SqlParameter("OptionName", cartProductOption.OptionName), 
                    new SqlParameter("OptionValue", cartProductOption.OptionValue), 
                    new SqlParameter("OptionDescription", cartProductOption.OptionDescription), 
                    new SqlParameter("PriceAlteration", cartProductOption.PriceAlteration), 
                };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, sqlParams);
        }
        public override void DeleteCartProductOption(int CartProductOptionId)
        {
            string delCmd = "DELETE FROM " + Prefix + "CartProductOption " +
                "WHERE CartProductOptionId = " + CartProductOptionId.ToString();
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);
        }
        public override void DeleteCartProductOptions(int CartProductId)
        {
            string delCmd = "DELETE FROM " + Prefix + "CartProductOption " +
                "WHERE CartProductId = " + CartProductId.ToString();
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);
        }

        // ProductTemplate methods
        public override IDataReader GetProductTemplate(int ProductTemplateId)
        {
            string selCmd = "SELECT *" +
                " FROM " + Prefix + "ProductTemplate ProductTemplate" +
                " WHERE ProductTemplateId = " + ProductTemplateId.ToString();
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetProductTemplate(int PortalId, int SubscriberId, string TemplateName, string TemplateSource)
        {
            string selCmd = "SELECT *" +
                " FROM " + Prefix + "ProductTemplate ProductTemplate" +
                " WHERE PortalId = " + PortalId.ToString() +
                (SubscriberId >= 0 ? " AND SubscriberId = " + SubscriberId.ToString() : "") +
                " AND TemplateName = N'" + TemplateName + "'" +
                " AND TemplateSource = N'" + TemplateSource + "'";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetProductTemplates(int PortalId, int SubscriberId, string TemplateSource)
        {
            string selCmd = "SELECT *" +
                " FROM " + Prefix + "ProductTemplate ProductTemplate" +
                " WHERE PortalId = " + PortalId.ToString() +
                (SubscriberId >= 0 ? " AND SubscriberId = " + SubscriberId.ToString() : "") +
                " AND TemplateSource = N'" + TemplateSource + "'";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetProductTemplates(int PortalId)
        {
            string selCmd = "SELECT *" +
                            " FROM " + GetFullyQualifiedName("ProductTemplate") +
                            " WHERE PortalId = " + PortalId.ToString(); 
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override int NewProductTemplate(ProductTemplateInfo ProductTemplate)
        {
            string insCmd = "set nocount on INSERT INTO " + Prefix + "ProductTemplate" +
                " (PortalId,SubscriberId,Templatename,Template,TemplateSource) VALUES" +
                " (" + ProductTemplate.PortalId.ToString() + "," +
                ProductTemplate.SubscriberId.ToString() + "," +
                "N'" + ProductTemplate.TemplateName + "'," +
                "N'" + ProductTemplate.Template + "'," +
                "N'" + ProductTemplate.TemplateSource + "')" +
                " SELECT CAST(scope_identity() AS INTEGER);";
            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd);
        }
        public override void UpdateProductTemplate(ProductTemplateInfo ProductTemplate)
        {
            string updCmd = "UPDATE " + Prefix + "ProductTemplate SET" +
                " PortalId = " + ProductTemplate.PortalId.ToString() + "," +
                " SubscriberId = " + ProductTemplate.SubscriberId.ToString() + "," +
                " TemplateName = N'" + ProductTemplate.TemplateName + "'," +
                " Template = N'" + ProductTemplate.Template + "'," +
                " TemplateSource = N'" + ProductTemplate.TemplateSource + "'" +
                " WHERE ProductTemplateId = " + ProductTemplate.ProductTemplateId.ToString();
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd);
        }
        public override void DeleteProductTemplate(int ProductTemplateId)
        {
            string delCmd = "DELETE FROM " + Prefix + "ProductTemplate WHERE ProductTemplateId = " + ProductTemplateId.ToString();
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);
        }

        // PaymentProvider methods
        public override IDataReader GetPaymentProvider(int PaymentProviderId, string Language)
        {
            string selCmd = "SELECT PaymentProvider.PaymentProviderId, PaymentProvider.ProviderLogo," +
                " PaymentProvider.ProviderTag, PaymentProvider.ProviderControl, Lang.ProviderName" +
                " FROM " + Prefix + "PaymentProvider PaymentProvider" +
                " INNER JOIN " + Prefix + "PaymentProviderLang Lang ON PaymentProvider.PaymentProviderId = Lang.PaymentProviderId" +
                " WHERE Lang.Language = '" + Language + "'" +
                " AND Paymentprovider.PaymentProviderId = " + PaymentProviderId.ToString();
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetPaymentProviders(string Language)
        {
            string selCmd = "SELECT PaymentProvider.PaymentProviderId, PaymentProvider.ProviderLogo," +
                " PaymentProvider.ProviderTag, PaymentProvider.ProviderControl, Lang.ProviderName" +
                " FROM " + Prefix + "PaymentProvider PaymentProvider" +
                " INNER JOIN " + Prefix + "PaymentProviderLang Lang ON PaymentProvider.PaymentProviderId = Lang.PaymentProviderId" +
                " WHERE Lang.Language = '" + Language + "'";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }

        // PaymentProviderLang methods
        public override IDataReader GetPaymentProviderLangs(int paymentProviderId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("PaymentProviderLang") +
                            " WHERE PaymentProviderId = @PaymentProviderId";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("PaymentProviderId", paymentProviderId));
        }
        public override void NewPaymentProviderLang(PaymentProviderLangInfo paymentProviderLang)
        {
            string sqlCmd = "INSERT INTO " + GetFullyQualifiedName("PaymentProviderLang") +
                            "(PaymentProviderId,Language,Providername) VALUES (@PaymentProviderId,@Language,@Providername)";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("PaymentProviderId",paymentProviderLang.PaymentProviderId), 
                                               new SqlParameter("Language",paymentProviderLang.Language),
                                               new SqlParameter("ProviderName", paymentProviderLang.ProviderName), 
                                           };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }


        // SubscriberPaymentProvider methods
        public override IDataReader GetSubscriberPaymentProviders(int PortalId, int SubscriberId)
        {
            string selCmd = "SELECT *" +
                " FROM " + Prefix + "SubscriberPaymentProvider PaymentProvider" +
                " WHERE PortalId = " + PortalId.ToString();
            if (SubscriberId != 0)
                selCmd += " AND SubscriberId = " + SubscriberId.ToString();
            selCmd += " ORDER BY ViewOrder";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetSubscriberPaymentProvider(int PortalId, int SubscriberId, int PaymentProviderId)
        {
            string selCmd = "SELECT *" +
                " FROM " + Prefix + "SubscriberPaymentProvider PaymentProvider" +
                " WHERE PortalId = " + PortalId.ToString();
            if (SubscriberId != 0)
                selCmd += " AND SubscriberId = " + SubscriberId.ToString();
            if (PaymentProviderId != 0)
                selCmd += " AND PaymentProviderId = " + PaymentProviderId.ToString();
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }

        public override IDataReader GetSubscriberPaymentProviderByCPP(int customerPaymentProviderId)
        {
            string selCmd = "SELECT spp.*" +
                            " FROM " + Prefix + "SubscriberPaymentProvider spp" +
                            " INNER JOIN " + GetFullyQualifiedName("CustomerPaymentProvider") + " cpp ON cpp.PaymentProviderId = spp.PaymentProviderId" +
                            " WHERE CustomerPaymentProviderId = @CustomerPaymentproviderId";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("CustomerPaymentProviderId",customerPaymentProviderId));
        }

        public override int NewSubscriberPaymentProvider(SubscriberPaymentProviderInfo SubscriberPaymentProvider)
        {
            string insCmd = "set nocount on INSERT INTO " + Prefix + "SubscriberPaymentProvider" +
                " (PortalId,SubscriberId,PaymentProviderId,ViewOrder,Cost,CostPercent,TaxPercent,Role,IsEnabled,PaymentProviderProperties) VALUES (" +
                SubscriberPaymentProvider.PortalId.ToString() + "," +
                SubscriberPaymentProvider.SubscriberId.ToString() + "," +
                SubscriberPaymentProvider.PaymentProviderId.ToString() + "," +
                SubscriberPaymentProvider.ViewOrder.ToString() + "," +
                SubscriberPaymentProvider.Cost.ToString(cult) + "," +
                SubscriberPaymentProvider.CostPercent.ToString(cult) + "," +
                SubscriberPaymentProvider.TaxPercent.ToString(cult) + "," +
                "N'" + SubscriberPaymentProvider.Role.ToString() + "'," +
                (SubscriberPaymentProvider.IsEnabled ? "1" : "0") + "," +
                "N'" + SubscriberPaymentProvider.PaymentProviderProperties + "')" +
                " SELECT CAST(scope_identity() AS INTEGER);";
            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd);
        }
        public override void UpdateSubscriberPaymentProvider(SubscriberPaymentProviderInfo SubscriberPaymentProvider)
        {
            string updCmd = "UPDATE " + Prefix + "SubscriberPaymentProvider SET " +
                " PortalId = " + SubscriberPaymentProvider.PortalId.ToString() + "," +
                " SubscriberId = " + SubscriberPaymentProvider.SubscriberId.ToString() + "," +
                " PaymentProviderId = " + SubscriberPaymentProvider.PaymentProviderId.ToString() + "," +
                " ViewOrder = " + SubscriberPaymentProvider.ViewOrder.ToString() + "," +
                " Cost = " + SubscriberPaymentProvider.Cost.ToString(cult) + "," +
                " CostPercent = " + SubscriberPaymentProvider.CostPercent.ToString(cult) + "," +
                " TaxPercent = " + SubscriberPaymentProvider.TaxPercent.ToString(cult) + "," +
                " Role = N'" + SubscriberPaymentProvider.Role.ToString() + "'," +
                " IsEnabled = " + (SubscriberPaymentProvider.IsEnabled ? "1" : "0") + "," +
                " PaymentProviderProperties = N'" + SubscriberPaymentProvider.PaymentProviderProperties + "'" +
                " WHERE SubscriberPaymentProviderId = " + SubscriberPaymentProvider.SubscriberPaymentProviderId.ToString();
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd);
        }
        public override void DeleteSubscriberPaymentProvider(int SubscriberPaymentProviderId)
        {
            string delCmd = "DELETE FROM " + Prefix + "SubscriberPaymentProvider" +
                " WHERE SubscriberPaymentProviderId = " + SubscriberPaymentProviderId;
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);
        }

        // CustomerPaymentProvider methods
        public override IDataReader GetCustomerPaymentProviders(int CustomerId)
        {
            string selCmd = "SELECT *" +
                " FROM " + Prefix + "CustomerPaymentProvider PaymentProvider" +
                " WHERE CustomerId = " + CustomerId.ToString();
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);

        }
        public override IDataReader GetCustomerPaymentProvider(int CustomerId, int PaymentProviderId)
        {
            string selCmd = "SELECT *" +
                " FROM " + Prefix + "CustomerPaymentProvider PaymentProvider" +
                " WHERE CustomerId = " + CustomerId.ToString() +
                " AND PaymentProviderId = " + PaymentProviderId.ToString();

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetCustomerPaymentProvider(int CustomerPaymentProviderId)
        {
            string selCmd = "SELECT *" +
                " FROM " + Prefix + "CustomerPaymentProvider PaymentProvider" +
                " WHERE CustomerPaymentProviderId = " + CustomerPaymentProviderId.ToString();

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override int NewCustomerPaymentProvider(CustomerPaymentProviderInfo CustomerPaymentProvider)
        {
            string insCmd = "set nocount on INSERT INTO " + Prefix + "CustomerPaymentProvider" +
                " (CustomerId,PaymentProviderId,PaymentProviderValues) VALUES (" +
                CustomerPaymentProvider.CustomerId.ToString() + "," +
                CustomerPaymentProvider.PaymentProviderId.ToString() + "," +
                "N'" + CustomerPaymentProvider.PaymentProviderValues + "')" +
                " SELECT CAST(scope_identity() AS INTEGER);";
            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd);

        }
        public override void UpdateCustomerPaymentProvider(CustomerPaymentProviderInfo CustomerPaymentProvider)
        {
            string updCmd = "UPDATE " + Prefix + "CustomerPaymentProvider SET " +
                " CustomerId = " + CustomerPaymentProvider.CustomerId.ToString() + "," +
                " PaymentProviderId = " + CustomerPaymentProvider.PaymentProviderId.ToString() + "," +
                " PaymentProviderValues = N'" + CustomerPaymentProvider.PaymentProviderValues + "'" +
                " WHERE CustomerPaymentProviderId = " + CustomerPaymentProvider.CustomerPaymentProviderId.ToString();
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd);

        }
        public override void DeleteCustomerPaymentProvider(int CustomerPaymentProviderId)
        {
            string delCmd = "DELETE FROM " + Prefix + "CustomerPaymentProvider" +
                " WHERE CustomerPaymentProviderId = " + CustomerPaymentProviderId;
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);
        }

        // SubscriberAddressType methods
        public override IDataReader GetSubscriberAddressTypes(int portalId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("SubscriberAddressType") + 
                            " WHERE PortalId = @PortalId " +
                            " ORDER BY ViewOrder";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("PortalId", portalId),
                                           };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override IDataReader GetSubscriberAddressTypes(int portalId, int subscriberId, string language)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("SubscriberAddressType") + " adr" +
                            " INNER JOIN " + GetFullyQualifiedName("SubscriberAddressTypeLang") +
                            " adrlang ON adr.SubscriberAddressTypeId = adrlang.SubscriberAddressTypeId" +
                            " WHERE adr.PortalId = @PortalId AND adr.SubscriberId = @SubscriberId and adrLang.Language = @Language"+
                            " ORDER BY adr.ViewOrder";
            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("PortalId", portalId),
                    new SqlParameter("SubscriberId", subscriberId),
                    new SqlParameter("Language", language), 
                };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override IDataReader GetSubscriberAddressType(int portalId, int subscriberId, string kzAddressType, string language)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("SubscriberAddressType") + " adr" +
                            " INNER JOIN " + GetFullyQualifiedName("SubscriberAddressTypeLang") +
                            " adrlang ON adr.SubscriberAddressTypeId = adrlang.SubscriberAddressTypeId" +
                            " WHERE adr.PortalId = @PortalId AND adr.SubscriberId = @SubscriberId "+
                            " AND  adr.kzAddressType = @KzAddressType AND adrLang.Language = @Language";
            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("PortalId", portalId),
                    new SqlParameter("SubscriberId", subscriberId),
                    new SqlParameter("KzAddressType", kzAddressType),
                    new SqlParameter("Language", language), 
                };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override IDataReader GetSubscriberAddressType(int subscriberAddressTypeId, string language)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("SubscriberAddressType") + " adr" +
                            " INNER JOIN " + GetFullyQualifiedName("SubscriberAddressTypeLang") +
                            " adrlang ON adr.SubscriberAddressTypeId = adrlang.SubscriberAddressTypeId" +
                            " WHERE adr.SubscriberAddressTypeId = @SubscriberAddressTypeId AND adrLang.Language = @Language";
            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("SubscriberAddressTypeId", subscriberAddressTypeId),
                    new SqlParameter("Language", language), 
                };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        public override int NewSubscriberAddressType(SubscriberAddressTypeInfo subscriberAddressType)
        {
            string sqlCmd = "SET NOCOUNT ON INSERT INTO " + GetFullyQualifiedName("SubscriberAddressType") +
                " (PortalId,SubscriberId,Mandatory,KzAddresstype,ViewOrder,IsOrderAddress) VALUES " +
                " (@PortalId,@SubscriberId,@Mandatory,@KzAddresstype,@ViewOrder,@IsOrderAddress) " +
                " SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("PortalId",subscriberAddressType.PortalId), 
                                               new SqlParameter("SubscriberId",subscriberAddressType.SubscriberId),
                                               new SqlParameter("Mandatory",subscriberAddressType.Mandatory),
                                               new SqlParameter("KzAddresstype",subscriberAddressType.KzAddressType),
                                               new SqlParameter("ViewOrder",subscriberAddressType.ViewOrder),
                                               new SqlParameter("IsOrderAddress",subscriberAddressType.IsOrderAddress),
                                           };
            return (int) SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override void UpdateSubscriberAddressType(SubscriberAddressTypeInfo subscriberAddressType)
        {
            string sqlCmd = "UPDATE " + GetFullyQualifiedName("SubscriberAddressType") + " SET" +
                            " PortalId = @PortalId," +
                            " SubscriberId = @SubscriberId," +
                            " Mandatory = @Mandatory," +
                            " KzAddresstype = @KzAddresstype," +
                            " ViewOrder = @Vieworder," +
                            " IsOrderAddress = @IsOrderAddress" +
                            " WHERE SubscriberAddressTypeId = @SubscriberAddressTypeId";

            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("SubscriberAddressTypeId",subscriberAddressType.SubscriberAddressTypeId),
                                               new SqlParameter("PortalId",subscriberAddressType.PortalId), 
                                               new SqlParameter("SubscriberId",subscriberAddressType.SubscriberId),
                                               new SqlParameter("Mandatory",subscriberAddressType.Mandatory),
                                               new SqlParameter("KzAddresstype",subscriberAddressType.KzAddressType),
                                               new SqlParameter("ViewOrder",subscriberAddressType.ViewOrder),
                                               new SqlParameter("IsOrderAddress",subscriberAddressType.IsOrderAddress),
                                           };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        // SubscriberAddressTypeLangs methods
        public override IDataReader GetSubscriberAddressTypeLangs(int subscriberAddressTypeId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("SubscriberAddressTypeLang") +
                            " WHERE SubscriberAddressTypeId = @SubscriberAddressTypeId";
            return (IDataReader) SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd,
                                        new SqlParameter("SubscriberAddressTypeId", subscriberAddressTypeId));
        }
        public override void NewSubscriberAddressTypeLang(SubscriberAddressTypeLangInfo subscriberAddressTypeLang)
        {
            string sqlCmd = "INSERT INTO " + GetFullyQualifiedName("SubscriberAddressTypeLang") +
                            "(SubscriberAddressTypeId,Language,AddressType) VALUES (@SubscriberAddressTypeId,@Language,@AddressType)";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("SubscriberAddressTypeId",
                                                                subscriberAddressTypeLang.SubscriberAddressTypeId),
                                               new SqlParameter("Language", subscriberAddressTypeLang.Language),
                                               new SqlParameter("AddressType", subscriberAddressTypeLang.AddressType),
                                           };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        

        // Order methods
        public override int SaveOrder(Guid CartId, int PortalId, string numberMask)
        {
            // TODO: Orderhistorie Insert
            int retVal = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    SqlTransaction trans = connection.BeginTransaction();
                    try
                    {
                        string sqlCmd = "SELECT MIN(OrderStateId) FROM " + GetFullyQualifiedName("OrderState") + " WHERE PortalId = @PortalId";
                        int orderStateId = (int)SqlHelper.ExecuteScalar(trans, CommandType.Text, sqlCmd, new SqlParameter("PortalId", PortalId));
                        
                        string insCmd = "set nocount on " +
                            "INSERT INTO " + Prefix + "Order (PortalId,SubscriberId,OrderNo,OrderTime,OrderName,OrderStateId,CustomerId," +
                            " Comment,Currency,Total,PaymentProviderId,PaymentProviderValues,Attachment, AttachName,AttachContentType) " +
                            " SELECT Cart.PortalId,Cart.SubscriberId,'abcde',GETDATE(),Cart.CartName," + orderStateId.ToString() + ",Cart.CustomerId," +
                            " Cart.Comment,Cart.Currency,Cart.Total,CustomerPaymentProvider.PaymentProviderId,CustomerPaymentProvider.PaymentProviderValues,Cart.Attachment,Cart.AttachName,Cart.AttachContentType " +
                            " FROM " + Prefix + "Cart Cart" +
                            " LEFT JOIN " + Prefix + "CustomerPaymentProvider CustomerPaymentProvider " +
                            " ON Cart.CustomerPaymentProviderID = CustomerPaymentProvider.CustomerPaymentProviderID" +
                            " WHERE Cart.CartID = '" + CartId.ToString() + "'" +
                            " SELECT CAST(scope_identity() AS INTEGER);";
                        int OrderId = (int)SqlHelper.ExecuteScalar(trans, CommandType.Text, insCmd);
                        retVal = OrderId;

                        // Lets determine the OrderNo
                        string orderNo = numberMask;
                        orderNo = orderNo.Replace("%D%", DateTime.Now.Day.ToString().PadLeft(2, '0'));
                        orderNo = orderNo.Replace("%M%", DateTime.Now.Month.ToString().PadLeft(2, '0'));
                        orderNo = orderNo.Replace("%Y2%", DateTime.Now.Year.ToString().Substring(2));
                        orderNo = orderNo.Replace("%Y4%", DateTime.Now.Year.ToString());
                        if (orderNo.IndexOf("%N") >= 0)
                        {
                            // Which is the next sequential number ?
                            int cntOrder = (int)SqlHelper.ExecuteScalar(trans, CommandType.Text, "SELECT Count(*) FROM " + Prefix + "Order WHERE PortalId = " + PortalId.ToString());
                            string strNumber = VfpInterop.StrExtract(orderNo, "%N", "%", 1, 1);
                            int digits;
                            if (Int32.TryParse(strNumber, out digits))
                                orderNo = orderNo.Replace("%N" + strNumber + "%", cntOrder.ToString().PadLeft(digits, '0'));
                        }

                        string updCmd = "UPDATE " + Prefix + "Order SET OrderNo = '" + orderNo + "' WHERE OrderId = " + OrderId.ToString();
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, updCmd);

                        insCmd = "INSERT INTO " + Prefix + "OrderAdditionalCost (OrderId, Quantity,Name,Description,UnitCost,TaxPercent,Area) " +
                            " SELECT " + OrderId.ToString() + ",Quantity,Name,Description,UnitCost,TaxPercent,Area " +
                            " FROM " + Prefix + "CartAdditionalCost " +
                            " WHERE CartId = '" + CartId.ToString() + "'";
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, insCmd);

                        string selCmd = "SELECT * FROM " + Prefix + "CartProduct " +
                            " WHERE CartId = '" + CartId.ToString() + "'";
                        DataSet ds = SqlHelper.ExecuteDataset(trans, CommandType.Text, selCmd);
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            insCmd = "set nocount on " +
                                     "INSERT INTO " + Prefix +
                                     "OrderProduct (OrderId,ProductId,Image,Unit,ItemNo,Quantity,Name,Description,UnitCost,TaxPercent) VALUES " +
                                     "(@OrderId,@ProductId,@Image,@Unit,@ItemNo,@Quantity,@Name,@Description,@UnitCost,@TaxPercent)"+
                                     " SELECT CAST(scope_identity() AS INTEGER);";

                            SqlParameter[] sqlParams = new SqlParameter[]
                                {
                                    new SqlParameter("OrderId",OrderId), 
                                    new SqlParameter("ProductId",dr["ProductId"]), 
                                    new SqlParameter("Image", dr["Image"]), 
                                    new SqlParameter("Unit", dr["Unit"]), 
                                    new SqlParameter("ItemNo", dr["ItemNo"]), 
                                    new SqlParameter("Quantity",dr["Quantity"]), 
                                    new SqlParameter("Name",dr["Name"]), 
                                    new SqlParameter("Description",dr["Description"]),
                                    new SqlParameter("UnitCost",dr["UnitCost"]),
                                    new SqlParameter("TaxPercent",dr["TaxPercent"]),
                                };
                                
                            int OrderProductId = (int)SqlHelper.ExecuteScalar(trans, CommandType.Text, insCmd, sqlParams);

                            insCmd = "INSERT INTO " + Prefix + "OrderProductOption (OrderProductId,OptionId,OptionName,OptionValue,OptionImage,OptionDescription,Pricealteration)" +
                                " SELECT " + OrderProductId.ToString() + ",OptionId,OptionName,OptionValue,OptionImage,OptionDescription,Pricealteration " +
                                " FROM " + Prefix + "CartProductOption" +
                                " WHERE CartProductId = " + dr["CartProductId"].ToString();
                            SqlHelper.ExecuteNonQuery(trans, CommandType.Text, insCmd);
                        }

                        insCmd = "INSERT INTO " + Prefix + "OrderAddress (OrderId,PortalId,SubscriberAddressTypeId,CustomerAddressId,Company,Prefix," +
                            " FirstName,MiddleName,LastName,Suffix,Unit,Street,Region,PostalCode,City,Suburb,Country,CountryCode," +
                            " Telephone,Cell,Fax,Email)" +
                            " SELECT " + OrderId.ToString() + ",CustomerAddress.PortalId,CartAddress.SubscriberAddressTypeId,CartAddress.CustomerAddressId,Company,Prefix," +
                            " FirstName,MiddleName,LastName,Suffix,Unit,Street,Region,PostalCode,City,Suburb,Country,CountryCode," +
                            " Telephone,Cell,Fax,Email " +
                            " FROM " + Prefix + "CustomerAddress CustomerAddress" +
                            " INNER JOIN " + Prefix + "CartAddress CartAddress ON CartAddress.CustomerAddressId =  CustomerAddress.CustomerAddressId" +
                            " WHERE CartAddress.CartId = '" + CartId.ToString() + "'";

                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, insCmd);

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        Exceptions.LogException(ex);
                        try
                        {
                            trans.Rollback();
                        }
                        catch (Exception exc)
                        {
                            Exceptions.LogException(exc);
                            throw;
                        }
                        retVal = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                retVal = -1;
            }
            return retVal;
        }
        public override IDataReader GetOrder(int OrderId)
        {
            string selCmd = "WITH Product as (SELECT Product.OrderProductID," +
                "Product.Quantity," +
                "Product.UnitCost," +
                "Product.TaxPercent," +
                "ISNULL((SELECT SUM(PriceAlteration )" +
                " FROM " + Prefix + "OrderProductOption ProductOption" +
                " WHERE ProductOption.OrderProductId = Product.OrderProductId),0.00)  as PriceAlteration" +
                " FROM " + Prefix + "OrderProduct Product WHERE Product.OrderId = " + OrderId.ToString() + ")," +
                "Additional as (SELECT Additional.OrderAdditionalCostId," +
                "Additional.Quantity," +
                "Additional.UnitCost," +
                "Additional.TaxPercent" +
                " FROM " + Prefix + "OrderAdditionalCost Additional WHERE Additional.OrderId = " + OrderId.ToString() + ")" +
                "SELECT Orders.*," +
                "ISNULL(( SELECT SUM(Product.Quantity * (Product.UnitCost + Product.PriceAlteration)) FROM Product ),0.00) as OrderTotal," +
                "ISNULL(( SELECT SUM(Product.Quantity * (Product.UnitCost + Product.PriceAlteration) * Product.Taxpercent / 100) FROM Product),0.00) as OrderTax," +
                "ISNULL(( SELECT SUM(Additional.Quantity * Additional.UnitCost) FROM Additional),0.00) as AdditionalTotal," +
                "ISNULL(( SELECT SUM(Additional.Quantity * Additional.UnitCost * Additional.Taxpercent / 100) FROM Additional),0.00) as AdditionalTax " +
                " FROM " + Prefix + "Order Orders " +
                " WHERE Orders.OrderId = " + OrderId.ToString();

            return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetOrders(int PortalId, string Language, string Sort, string Filter)
        {
            string selCmd = "SELECT DISTINCT Orders.*," +
                            " ISNULL(OrderAddress.FirstName,'') as Firstname,"+
                            " ISNULL(OrderAddress.LastName,'') as Lastname,"+
                            " ISNULL(OrderAddress.Company,'') as Company,"+
                            " ISNULL(OrderAddress.Street,'') as Street," +
                            " ISNULL(OrderAddress.PostalCode,'') as PostalCode," +
                            " ISNULL(OrderAddress.City,'') as City, " +
                            " ISNULL(OrderAddress.CountryCode,'') as CountryCode, " +
                            " ISNULL((SELECT ProviderName FROM dbo.BBStore_PaymentProviderLang WHERE PaymentProviderId = Orders.PaymentProviderId AND Language = @Language) ,'') as PaymentProvider, " +
                            " OrderStateLang.OrderState" +
                            " FROM " + GetFullyQualifiedName("Order") + " Orders " +
                            " LEFT JOIN " + GetFullyQualifiedName("OrderAddress") + " OrderAddress ON Orders.OrderId = OrderAddress.OrderId" +
                            " LEFT JOIN " + GetFullyQualifiedName("SubscriberAddressType") + " SubscriberAddressType ON SubscriberAddressType.SubscriberAddressTypeId = OrderAddress.SubscriberAddressTypeId" +
                            " LEFT JOIN " + GetFullyQualifiedName("OrderState") + " OrderState ON Orders.OrderStateId = OrderState.OrderStateId" +
                            " LEFT JOIN " + GetFullyQualifiedName("OrderStateLang") + " OrderStateLang ON Orders.OrderStateId = OrderStateLang.OrderStateId" +
                            " WHERE SubscriberAddressType.IsOrderAddress = 1" + 
                            " AND OrderStateLang.Language = @Language" + 
                            " AND OrderState.PortalId = @PortalId" +
                            " AND Orders.PortalId = @PortalId";

            if (!string.IsNullOrEmpty(Filter))
                selCmd += " AND " + Filter;
            if (!string.IsNullOrEmpty(Sort))
                selCmd += " ORDER BY " + Sort + " DESC";

            SqlParameter[] SqlParams = new SqlParameter[]
                                        {
                                            new SqlParameter("PortalId", PortalId),
                                            new SqlParameter("Language", Language)
                                        };

            return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetOrderProducts(int OrderId)
        {
            string selCmd = "SELECT OrderProduct.OrderProductId, OrderProduct.OrderID,OrderProduct.ProductID,OrderProduct.Image, OrderProduct.ItemNo," +
                 " OrderProduct.Name, OrderProduct.Quantity,OrderProduct.TaxPercent, Orderproduct.Unit, OrderProduct.Description," +
                 " OrderProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0) AS UnitCost," +
                 " OrderProduct.Quantity * ROUND((OrderProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)) * (100 + OrderProduct.TaxPercent) / 100,2) AS SubTotal," +
                 " ROUND(OrderProduct.Quantity * (OrderProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)),2) As NetTotal," +
                 " OrderProduct.Quantity * ROUND((OrderProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)) * (100 + OrderProduct.TaxPercent) / 100,2) - " +
                 " ROUND(OrderProduct.Quantity * (OrderProduct.UnitCost + ISNULL(SUM(ProductOption.PriceAlteration),0)),2) AS TaxTotal" +
                 " FROM " + Prefix + "OrderProduct OrderProduct" +
                 " LEFT JOIN " + Prefix + "OrderProductOption ProductOption ON ProductOption.OrderProductID = OrderProduct.OrderProductID" +
                 " WHERE OrderProduct.OrderId = " + OrderId.ToString() +
                 " GROUP BY OrderProduct.OrderProductId, OrderProduct.OrderID,OrderProduct.ProductID,OrderProduct.Image, OrderProduct.ItemNo," +
                 " OrderProduct.Name, OrderProduct.Quantity,OrderProduct.TaxPercent,OrderProduct.Unit,OrderProduct.UnitCost, OrderProduct.Description";
            return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetOrderProductOptions(int OrderProductId)
        {
            string selCmd = "SELECT * FROM " + Prefix + "OrderProductOption " +
                "WHERE OrderProductId = " + OrderProductId.ToString();
            return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetOrderAdditionalCosts(int OrderId)
        {
            string selCmd = "SELECT *," +
                " Quantity * UnitCost As NetTotal," +
                " Quantity * UnitCost * TaxPercent / 100 AS TaxTotal," +
                " Quantity * UnitCost * (100 + TaxPercent) / 100 AS SubTotal" +
                " FROM " + Prefix + "OrderAdditionalCost OrderAdditionalCost" +
                " WHERE OrderId = " + OrderId.ToString();

            return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetOrderAddresses(int orderId, string language)
        {
            string sqlCmd = "SELECT oa.*, ISNULL(satl.AddressType,'') as AddressType " +
                            " FROM " + GetFullyQualifiedName("OrderAddress") + " oa " +
                            " LEFT JOIN " + GetFullyQualifiedName("SubscriberAddressTypeLang") +
                            " satl ON oa.SubscriberAddressTypeId = satl.SubscriberAddressTypeId" +
                            " WHERE oa.OrderId = @OrderId AND satl.Language=@language";

            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("OrderId",orderId), 
                    new SqlParameter("Language",language), 
                };
            
            return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override bool HasOrderAddress(int customerAddressId)
        {
            string sqlCmd = "SELECT COUNT(*) FROM " + GetFullyQualifiedName("OrderAddress") +
                            " WHERE CustomerAddressId = @CustomerAddressId";
            int anz = (int) SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("CustomerAddressId", customerAddressId));
            return (anz > 0);
        }


        // OrderStates methods
        public override IDataReader GetOrderStates(int portalId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("OrderState") + " OrderState" +
                " WHERE PortalId = @PortalId";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("PortalId",portalId));

        }

        public override IDataReader GetOrderStates(int portalId, string language)
        {
            string selCmd = "SELECT DISTINCT OrderState.OrderStateId, OrderState.OrderAction, OrderState.PortalId, Lang.OrderState" +
                " FROM " + GetFullyQualifiedName("OrderState") + " OrderState" +
                " INNER JOIN " + GetFullyQualifiedName("OrderStateLang") + " Lang ON OrderState.OrderStateId = Lang.OrderStateId" + 
                " WHERE Lang.Language = @Language" +
                " AND OrderState.PortalId = @PortalId";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("PortalId",portalId), new SqlParameter("Language",language));
        }

        public override void SetOrderState(int orderId,int orderStateId)
        {
            string sqlCmd = "UPDATE " + GetFullyQualifiedName("Order") + " SET" +
                            " OrderStateId = @OrderStateId" +
                            " WHERE OrderId = @OrderId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("OrderId", orderId),
                                      new SqlParameter("OrderStateId", orderStateId));
        }
        public override int NewOrderState(OrderStateInfo orderState)
        {
            string sqlCmd = "SET NOCOUNT ON INSERT INTO "+ GetFullyQualifiedName("OrderState") +
                " (OrderAction, PortalId) VALUES (@OrderAction,@PortalId)" +
                " SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("PortalId", orderState.PortalId),
                                               new SqlParameter("OrderAction", orderState.OrderAction)
                                           };
            return (int) SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd,sqlParams);
        }

        // OrderStateLang methods
        public override IDataReader GetOrderStateLangs(int orderStateId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("OrderStateLang") +
                " WHERE OrderStateId = @OrderStateId";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("OrderStateId", orderStateId));
        }
        public override IDataReader GetOrderStateLang(int orderStateId, string language)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("OrderStateLang") +
                " WHERE OrderStateId = @OrderStateId" +
                " And Language = @Language";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("OrderStateId", orderStateId),
                new	SqlParameter("Language",language)};
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override void NewOrderStateLang(OrderStateLangInfo orderStateLang)
        {
            string insCmd = "INSERT INTO " + GetFullyQualifiedName("OrderStateLang") +
                " (OrderStateId,Language,OrderState)" +
                " VALUES " +
                " (@OrderStateId,@Language,@OrderState)";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("OrderStateId",orderStateLang.OrderStateId),
                new SqlParameter("Language",orderStateLang.Language),
                new SqlParameter("OrderState",orderStateLang.OrderState)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateOrderStateLang(OrderStateLangInfo orderStateLang)
        {
            string updCmd = "UPDATE " + GetFullyQualifiedName("OrderStateLang") + " SET " +
                " OrderState = @OrderState" +
                " WHERE Language = @Language" +
                " AND OrderStateId = @OrderStateId";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("OrderStateId",orderStateLang.OrderStateId),
                new SqlParameter("Language",orderStateLang.Language),
                new SqlParameter("OrderState",orderStateLang.OrderState)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteOrderStateLangs(int orderStateId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("OrderStateLang") +
                " WHERE OrderStateId = @OrderStateId;";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("OrderStateId", orderStateId));
        }
        public override void DeleteOrderStateLang(int orderStateId, string language)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("OrderStateLang") +
                " WHERE OrderStateId = @OrderStateId" +
                " AND Language = @Language";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("OrderStateId",orderStateId),
                new SqlParameter("Language",language)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
        }

        // ProductGroup methods
        public override IDataReader GetProductGroups(int portalId)
        {
            string selCmd = "SELECT *" +
                            " FROM " + GetFullyQualifiedName("ProductGroup") + " ProductGroup" +
                            " WHERE PortalId = @PortalId";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("PortalId", portalId));
        }
        public override IDataReader GetProductGroups(int PortalId, string Language, bool includeDisabled)
        {
            string selCmd = "SELECT ProductGroup.ProductGroupId, ProductGroup.ParentId, ProductGroup.SubscriberId," +
                " ProductGroup.PortalId, ProductGroup.Image, ProductGroup.Icon,Lang.ProductGroupName,Lang.ProductGroupShortDescription,"+
                " Lang.ProductGroupDescription,ProductGroup.ProductListTabId," +
                " 0 as ProductCount, ProductGroup.Disabled,ProductGroup.ViewOrder" +
                " FROM " + Prefix + "ProductGroup ProductGroup" +
                " INNER JOIN " + Prefix + "ProductGroupLang Lang ON ProductGroup.ProductGroupId = Lang.ProductGroupId" +
                " WHERE ProductGroup.PortalId = " + PortalId.ToString() +
                (includeDisabled ? "" : " AND ProductGroup.Disabled = 0") +
                " AND Lang.Language = '" + Language + "'";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetProductSubGroupsByNode(int PortalId, string Language, int NodeId, bool IncludeCount, bool IncludeSubDirsInCount, bool IncludeDisabled)
        {
            string selCmd;
            if (IncludeCount)
                if (IncludeSubDirsInCount)
                {
                    selCmd = "with x as " +
                            "      (SELECT ProductGroup.ProductGroupId as Id, " +
                            "		ProductGroup.ParentId as IDRef," +
                            "		(SELECT COUNT(*) FROM " + GetFullyQualifiedName("SimpleProduct") + " SimpleProduct" +
                            "		  INNER JOIN " + GetFullyQualifiedName("ProductInGroup") + " ProductInGroup " +
                            "		  ON SimpleProduct.SimpleProductId = ProductInGroup.SimpleProductId" +
                            "		  WHERE ProductInGroup.ProductGroupId = ProductGroup.ProductGroupId"+
                            (IncludeDisabled ? "" : " AND SimpleProduct.Disabled = 0") +") As ProductCount" +
                            "	  FROM " + GetFullyQualifiedName("ProductGroup") + " ProductGroup" +
                            "     WHERE ProductGroup.PortalId = " + PortalId.ToString() + ")," +
                            "	y as (SELECT x.id AS Root, x.id AS IDRef, x.ProductCount, x.ID FROM X " +
                            "		  UNION all" +
                            "		  SELECT y.Root, x.IDRef, x.ProductCount, x.ID FROM y INNER join X ON x.IDRef = y.ID)," +
                            "	z as (SELECT Root, SUM(ProductCount) AS ProductCount FROM y GROUP BY Root)" +
                            "SELECT ProductGroup.ProductGroupId, ProductGroup.ParentId, ProductGroup.SubscriberId," +
                            "ProductGroup.PortalId, ProductGroup.Image, ProductGroup.Icon,Lang.ProductGroupName,Lang.ProductGroupShortDescription,"+
							"Lang.ProductGroupDescription,ProductGroup.ProductListTabId," +
                            " z.ProductCount,ProductGroup.Disabled,ProductGroup.ViewOrder" +
                            " FROM " + GetFullyQualifiedName("ProductGroup") + " ProductGroup" +
                            " INNER JOIN " + GetFullyQualifiedName("ProductGroupLang") + " Lang ON ProductGroup.ProductGroupId = Lang.ProductGroupId" +
                            " INNER JOIN z ON ProductGroup.ProductGroupId = z.Root" +
                            " WHERE Lang.Language = '" + Language + "'" +
                            " AND ProductGroup.PortalId = " + PortalId.ToString() +
                            " AND ProductGroup.ParentId " + (NodeId == -1 ? " IS NULL" : " = " + NodeId.ToString());
                    if (!IncludeDisabled)
                        selCmd += " AND ProductGroup.Disabled = 0";

                    selCmd += " ORDER BY ViewOrder,ProductGroupName OPTION (MaxRecursion 10)";
                }
                else
                {
                    selCmd = "SELECT ProductGroup.ProductGroupId, ProductGroup.ParentId, ProductGroup.SubscriberId," +
						" ProductGroup.PortalId, ProductGroup.Image, ProductGroup.Icon,Lang.ProductGroupName,Lang.ProductGroupShortDescription," +
						" Lang.ProductGroupDescription,ProductGroup.ProductListTabId," +
                        " (SELECT COUNT(*) FROM " + GetFullyQualifiedName("SimpleProduct") + " SimpleProduct" +
                        "  INNER JOIN " + GetFullyQualifiedName("ProductInGroup") + " ProductInGroup " +
                        "  ON SimpleProduct.SimpleProductId = ProductInGroup.SimpleProductId" +
                        "  WHERE ProductInGroup.ProductGroupId = ProductGroup.ProductGroupId" + 
                        (IncludeDisabled ? "" : " AND SimpleProduct.Disabled = 0")  + ") As ProductCount," +
                        " ProductGroup.Disabled,ProductGroup.ViewOrder" +
                        " FROM " + Prefix + "ProductGroup ProductGroup" +
                        " INNER JOIN " + Prefix + "ProductGroupLang Lang ON ProductGroup.ProductGroupId = Lang.ProductGroupId" +
                        " WHERE Lang.Language = '" + Language + "'" +
                        " AND ProductGroup.PortalId = " + PortalId.ToString() +
                        " AND ProductGroup.ParentId " + (NodeId == -1 ? " IS NULL" : " = " + NodeId.ToString());
                    if (!IncludeDisabled)
                        selCmd += " AND ProductGroup.Disabled = 0";
                    selCmd += " ORDER BY ViewOrder,ProductGroupName";
                }
            else
            {
                selCmd = "SELECT ProductGroup.ProductGroupId, ProductGroup.ParentId, ProductGroup.SubscriberId," +
					" ProductGroup.PortalId, ProductGroup.Image, ProductGroup.Icon,Lang.ProductGroupName,Lang.ProductGroupShortDescription," +
					" Lang.ProductGroupDescription,ProductGroup.ProductListTabId," +
                    " 0 as ProductCount,ProductGroup.Disabled,ProductGroup.ViewOrder" +
                    " FROM " + Prefix + "ProductGroup ProductGroup" +
                    " INNER JOIN " + Prefix + "ProductGroupLang Lang ON ProductGroup.ProductGroupId = Lang.ProductGroupId" +
                    " WHERE Lang.Language = '" + Language + "'" +
                    " AND ProductGroup.PortalId = " + PortalId.ToString() +
                    " AND ProductGroup.ParentId " + (NodeId == -1 ? " IS NULL" : " = " + NodeId.ToString());
                if (!IncludeDisabled)
                    selCmd += " AND ProductGroup.Disabled = 0";
                selCmd += " ORDER BY ViewOrder,ProductGroupName";
            }

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetProductGroupByName(int PortalId, string Language, string ProductGroupName)
        {
            string selCmd = "SELECT ProductGroup.ProductGroupId, ProductGroup.ParentId, ProductGroup.SubscriberId," +
                 " ProductGroup.PortalId, ProductGroup.Image, ProductGroup.Icon,Lang.ProductGroupName,Lang.ProductGroupShortDescription," +
                 " Lang.ProductGroupDescription,ProductGroup.ProductListTabId," +
                 " 0 as ProductCount,ProductGroup.Disabled,ProductGroup.ViewOrder" +
                 " FROM " + GetFullyQualifiedName("ProductGroup") + "  ProductGroup" +
                 " INNER JOIN " + GetFullyQualifiedName("ProductGroupLang") + " Lang ON ProductGroup.ProductGroupId = Lang.ProductGroupId" +
                 " WHERE ProductGroup.PortalId = @PortalId" + 
                 " AND Lang.Language = @Language " +
                 " AND Lang.ProductGroupName = @ProductGroupName";

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("PortalId",PortalId),
                new SqlParameter("Language",Language),
                new SqlParameter("ProductGroupName",ProductGroupName) };

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, param);
        }
        public override IDataReader GetProductGroup(int PortalId, string Language, int ProductGroupId)
        {
            string selCmd = "SELECT ProductGroup.ProductGroupId, ProductGroup.ParentId, ProductGroup.SubscriberId," +
                 " ProductGroup.PortalId, ProductGroup.Image, ProductGroup.Icon,Lang.ProductGroupName,Lang.ProductGroupShortDescription," +
                 " Lang.ProductGroupDescription,ProductGroup.ProductListTabId," +
                 " 0 as ProductCount,ProductGroup.Disabled,ProductGroup.ViewOrder" +
                 " FROM " + Prefix + "ProductGroup ProductGroup" +
                 " INNER JOIN " + Prefix + "ProductGroupLang Lang ON ProductGroup.ProductGroupId = Lang.ProductGroupId" +
                 " WHERE ProductGroup.PortalId = " + PortalId.ToString() +
                 " AND Lang.Language = '" + Language + "'" +
                 " AND ProductGroup.ProductGroupId = " + ProductGroupId.ToString();
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetProductGroup(int PortalId, int ProductGroupId)
        {
            string selCmd = "SELECT ProductGroup.ProductGroupId, ProductGroup.ParentId, ProductGroup.SubscriberId," +
                 " ProductGroup.PortalId, ProductGroup.Image, ProductGroup.Icon, '' As ProductGroupName,ProductGroup.ProductListTabId," +
                 " 0 as ProductCount,ProductGroup.Disabled,ProductGroup.ViewOrder" +
                 " FROM " + GetFullyQualifiedName("ProductGroup") + " ProductGroup" +
                 " WHERE ProductGroup.PortalId = " + PortalId.ToString() +
                 " AND ProductGroup.ProductGroupId = " + ProductGroupId.ToString();
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }

        public override string GetProductGroupPath(int PortalId, int ProductGroupId)
        {
            return GetProductGroupPath(PortalId, ProductGroupId, "en-US", true, "/", "", "");
        }
        public override string GetProductGroupPath(int PortalId, int ProductGroupId, string Language, bool returnId, string Delimiter, string linkTemplate, string rootText)
        {
            string retVal = "";
            string selCmd;
            object zosn;

			if (linkTemplate == String.Empty)
				linkTemplate = "{0}";

            if (ProductGroupId == -1)
                return retVal;
            else
            {
                if (returnId)
                    retVal = "_" + ProductGroupId.ToString();
                else
                {
                    selCmd = "SELECT ProductGroupName FROM " + Prefix + "ProductGroupLang ProductGroupLang" +
                        " WHERE ProductGroupLang.ProductGroupId = " + ProductGroupId.ToString() +
                        " AND ProductGroupLang.Language = '" + Language + "'";
                    zosn = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd);
                    if (zosn != DBNull.Value)
                        retVal = (string)zosn;
                }
            }

            if (returnId)
                selCmd = "SELECT ProductGroup.ParentId,'' as ProductGroupName" +
                    " FROM " + Prefix + "ProductGroup ProductGroup" +
                    " WHERE ProductGroup.PortalId = " + PortalId.ToString() +
                    " AND ProductGroup.ProductGroupId = ";
            else
                selCmd = "SELECT ProductGroup.ParentId,ISNULL(ProductGroupLang.ProductGroupName,'') as ProductGroupName" +
                    " FROM " + Prefix + "ProductGroup ProductGroup" +
                    " LEFT JOIN " + Prefix + "ProductGroupLang ProductGroupLang ON ProductGroup.ParentId = ProductGroupLang.ProductGroupId" +
                    " WHERE ProductGroup.PortalId = " + PortalId.ToString() +
                    " AND ProductGroupLang.Language = '" + Language + "'" +
                    " AND ProductGroup.ProductGroupId = ";
            IDataReader blisn = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd + ProductGroupId.ToString());
            if (blisn.Read())
            {
                int ParentId = (blisn["ParentId"] != DBNull.Value ? (int)blisn["ParentId"] : -1);
                if (ParentId > -1)
                {
                    if (returnId)
                        retVal = "_" + ParentId.ToString() + Delimiter + retVal;
                    else
                        retVal = String.Format(linkTemplate,blisn["ProductGroupName"],blisn["ParentId"]) + Delimiter + retVal;
                }
                while (ParentId != -1)
                {
                    blisn = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd + ParentId.ToString());
                    if (blisn.Read())
                    {
                        ParentId = (blisn["ParentId"] != DBNull.Value ? (int)blisn["ParentId"] : -1);
                        if (ParentId > -1)
                        {
                            if (returnId)
                                retVal = "_" + ParentId.ToString() + Delimiter + retVal;
                            else
								retVal = String.Format(linkTemplate, blisn["ProductGroupName"], blisn["ParentId"]) + Delimiter + retVal;
                        }
                    }
                    else
                        ParentId = -1;
                }
            }
            if (!returnId && !String.IsNullOrEmpty(rootText))
                retVal = String.Format(linkTemplate, rootText, -1) + Delimiter + retVal;
            return retVal;
        }
        public override int NewProductGroup(ProductGroupInfo ProductGroup)
        {
            string insCmd = "SET NOCOUNT ON INSERT INTO " + Prefix + "ProductGroup " +
               " (ParentId,SubscriberId,PortalId,Image,Icon," +
               "  ProductListTabId,Disabled,ViewOrder)" +
               " VALUES " +
               " (@ParentId,@SubscriberId,@PortalId,@Image,@Icon," +
               "  @ProductListTabId,@Disabled,@ViewOrder) SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter[] SqlParams = new SqlParameter[] {
               new SqlParameter("ParentId",GetNull(ProductGroup.ParentId)),
               new SqlParameter("SubscriberId",ProductGroup.SubscriberId),
               new SqlParameter("PortalId",ProductGroup.PortalId),
               new SqlParameter("Image",ProductGroup.Image),
               new SqlParameter("Icon",ProductGroup.Icon),
               new SqlParameter("Disabled",ProductGroup.Disabled),
               new SqlParameter("ViewOrder",ProductGroup.ViewOrder),
               new SqlParameter("ProductListTabId",GetNull(ProductGroup.ProductListTabId)) };

            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateProductGroup(ProductGroupInfo ProductGroup)
        {
            string updCmd = "UPDATE " + Prefix + "ProductGroup SET " +
                " ParentId = @ParentId," +
                " SubscriberId = @SubscriberId," +
                " PortalId = @PortalId," +
                " Image = @Image," +
                " Icon = @Icon," +
                " Disabled = @Disabled," +
                " ViewOrder = @ViewOrder," +
                " ProductListTabId = @ProductListTabId" +
                " WHERE ProductGroupId = @ProductGroupId";

            SqlParameter[] SqlParams = new SqlParameter[] {
               new SqlParameter("ProductGroupId",ProductGroup.ProductGroupId),
               new SqlParameter("ParentId",GetNull(ProductGroup.ParentId)),
               new SqlParameter("SubscriberId",ProductGroup.SubscriberId),
               new SqlParameter("PortalId",ProductGroup.PortalId),
               new SqlParameter("Image",ProductGroup.Image),
               new SqlParameter("Icon",ProductGroup.Icon),
               new SqlParameter("Disabled",ProductGroup.Disabled),
               new SqlParameter("ViewOrder",ProductGroup.ViewOrder),
               new SqlParameter("ProductListTabId",GetNull(ProductGroup.ProductListTabId)) };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteProductGroup(int ProductGroupId)
        {
            string delCmd = "DELETE FROM " + Prefix + "ProductGroup " +
                " WHERE ProductGroupId = @ProductGroupId";

            SqlParameter[] SqlParams = new SqlParameter[]{
               new SqlParameter("ProductGroupId",ProductGroupId)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
        }
        public override void DeleteProductGroups(int PortalId)
        {
            string delCmd = "DELETE FROM " + Prefix + "ProductGroup " +
                " WHERE PortalId = @PortalId";

            SqlParameter[] SqlParams = new SqlParameter[]{
               new SqlParameter("PortalId",PortalId)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
        }

        // ProductGroupLang methods
        public override IDataReader GetProductGroupLang(int ProductGroupId, string Language)
        {
            string selCmd = "SELECT * " +
                " FROM " + Prefix + "ProductGroupLang ProductGroupLang" +
                " WHERE ProductGroupLang.ProductGroupId = @ProductGroupId" +
                " AND Language = @Language";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("ProductGroupId",ProductGroupId),
                new SqlParameter("Language",Language)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetProductGroupLangs(int ProductGroupId)
        {
            string selCmd = "SELECT * " +
                " FROM " + Prefix + "ProductGroupLang ProductGroupLang" +
                " WHERE ProductGroupLang.ProductGroupId = @ProductGroupId";

            SqlParameter[] SqlParams = new SqlParameter[] {
               new SqlParameter("ProductGroupId",ProductGroupId)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetProductGroupLangsByPortal(int portalId)
        {
            string selCmd = "SELECT * " +
                " FROM " + GetFullyQualifiedName("ProductGroupLang") +
                " WHERE ProductGroupId IN (SELECT ProductGroupId FROM " + GetFullyQualifiedName("ProductGroup") + " WHERE PortalId = @PortalId)";

            SqlParameter[] SqlParams = new SqlParameter[] {
               new SqlParameter("PortalId",portalId)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }

        public override void NewProductGroupLang(ProductGroupLangInfo ProductGroupLang)
        {
            string insCmd = "INSERT INTO " + Prefix + "ProductGroupLang " +
               " (ProductGroupId," +
               "  Language,ProductGroupName,ProductGroupShortDescription,ProductGroupDescription)" +
               " VALUES " +
               " (@ProductGroupId," +
               "  @Language,@ProductGroupName,@ProductGroupShortDescription,@ProductGroupDescription)";

            SqlParameter[] SqlParams = new SqlParameter[] {
               new SqlParameter("ProductGroupId",ProductGroupLang.ProductGroupId),
               new SqlParameter("Language",ProductGroupLang.Language),
               new SqlParameter("ProductGroupName",ProductGroupLang.ProductGroupName),
               new SqlParameter("ProductGroupShortDescription",ProductGroupLang.ProductGroupShortDescription),
               new SqlParameter("ProductGroupDescription",ProductGroupLang.ProductGroupDescription),
            };


            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateProductGroupLang(ProductGroupLangInfo ProductGroupLang)
        {
            string updCmd = "UPDATE " + Prefix + "ProductGroupLang SET " +
            " ProductGroupName = @ProductGroupName," +
            " ProductGroupShortDescription = @ProductGroupShortDescription," +
            " ProductGroupDescription = @ProductGroupDescription" +
            " WHERE ProductGroupId = @ProductGroupId" +
            " AND Language = @Language";

            SqlParameter[] SqlParams = new SqlParameter[]
                {
                    new SqlParameter("ProductGroupId", ProductGroupLang.ProductGroupId),
                    new SqlParameter("Language", ProductGroupLang.Language),
                    new SqlParameter("ProductGroupName", ProductGroupLang.ProductGroupName),
                    new SqlParameter("ProductGroupShortDescription", ProductGroupLang.ProductGroupShortDescription),
                    new SqlParameter("ProductGroupDescription", ProductGroupLang.ProductGroupDescription)
                };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);

        }
        public override void DeleteProductGroupLangs(int ProductGroupId)
        {
            string delCmd = "DELETE FROM " + Prefix + "ProductGroupLang" +
                " WHERE ProductGroupId = @ProductGroupId";

            SqlParameter[] SqlParams = new SqlParameter[] {
               new SqlParameter("ProductGroupId",ProductGroupId)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
        }
        public override void DeleteProductGroupLang(int ProductGroupId, string Language)
        {
            string delCmd = "DELETE FROM " + Prefix + "ProductGroupLang" +
                " WHERE ProductGroupId = @ProductGroupId" +
                " AND Language = @Language";

            SqlParameter[] SqlParams = new SqlParameter[] {
               new SqlParameter("ProductGroupId",ProductGroupId),
               new SqlParameter("Language",Language)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
        }


        // ProductInGroup methods
        public override DataTable GetProductsInGroupByProduct(int SimpleProductId)
        {
            string selCmd = "SELECT ProductGroupId FROM " + Prefix + "ProductInGroup WHERE SimpleProductId = " + SimpleProductId.ToString();
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, selCmd);
            return ds.Tables[0];

        }
        public override IDataReader GetProductsInGroupByPortal(int portalId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("ProductInGroup") + 
                " WHERE SimpleProductId IN (SELECT SimpleProductId FROM  " + GetFullyQualifiedName("SimpleProduct") + " WHERE PortalId = @PortalId)";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("PortalId", portalId));
        }

        public override void NewProductInGroup(int SimpleProductId, int ProductGroupId)
        {
            string insCmd = "INSERT INTO " + Prefix + "ProductInGroup (SimpleProductId,ProductGroupId) VALUES (" +
                SimpleProductId.ToString() + "," +
                ProductGroupId.ToString() + ")";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd);

        }
        public override void DeleteProductInGroups(int SimpleProductId)
        {
            string delCmd = "DELETE FROM " + Prefix + "ProductInGroup WHERE SimpleProductId = " + SimpleProductId.ToString();
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);
        }
        public override void DeleteProductInGroup(int SimpleProductId, int ProductGroupId)
        {
            string delCmd = "DELETE FROM " + Prefix + "ProductInGroup " +
                " WHERE SimpleProductId = @SimpleProductId AND ProductGroupId = @ProductGroupId";

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("SimpleProductId",SimpleProductId),
                new SqlParameter("ProductGroupId",ProductGroupId)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd,param);
        }

        //ProductFilter methods
        public override IDataReader GetProductFilters(int PortalId, Guid FilterSessionId)
        {
            string selCmd = "SELECT * FROM " + Prefix + "ProductFilter ProductFilter" +
                " WHERE PortalId = @PortalId" +
                " AND FilterSessionId = @FilterSessionId";
            return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd,
                new SqlParameter("PortalId", PortalId),
                new SqlParameter("FilterSessionId", FilterSessionId));

        }
        public override IDataReader GetProductFilter(int PortalId, Guid FilterSessionId, string FilterSource)
        {
            string selCmd = "SELECT * FROM " + Prefix + "ProductFilter ProductFilter" +
                " WHERE PortalId = @PortalId" +
                " AND FilterSessionId = @FilterSessionId" +
                " AND FilterSource = @FilterSource";
            return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd,
                new SqlParameter("PortalId", PortalId),
                new SqlParameter("FilterSessionId", FilterSessionId),
                new SqlParameter("FilterSource", FilterSource));

        }
        public override void NewProductFilter(ProductFilterInfo ProductFilter)
        {
            if (ProductFilter.FilterValue != string.Empty)
            {
                string insCmd = "INSERT INTO " + Prefix + "ProductFilter " +
                    "(FilterSessionId,PortalId,FilterSource,FilterValue,FilterCondition)" +
                    " VALUES " +
                    "(@FilterSessionId,@PortalId,@FilterSource,@FilterValue,@FilterCondition)";
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd,
                    new SqlParameter("FilterSessionid", ProductFilter.FilterSessionId),
                    new SqlParameter("PortalId", ProductFilter.PortalId),
                    new SqlParameter("FilterSource", ProductFilter.FilterSource),
                    new SqlParameter("FilterValue", ProductFilter.FilterValue),
                    new SqlParameter("FilterCondition", ProductFilter.FilterCondition));
            }

        }
        public override void UpdateProductFilter(ProductFilterInfo ProductFilter)
        {
            string updCmd = "UPDATE " + Prefix + "ProductFilter SET " +
                " FilterSource = @FilterSource," +
                " FilterValue = @Filtervalue," +
                " FilterCondition = @FilterCondition" +
                " WHERE FilterSessionId = @FilterSessionId" +
                " AND PortalId = @PortalId";

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd,
                new SqlParameter("FilterSessionid", ProductFilter.FilterSessionId),
                new SqlParameter("PortalId", ProductFilter.PortalId),
                new SqlParameter("FilterSource", ProductFilter.FilterSource),
                new SqlParameter("FilterValue", ProductFilter.FilterValue),
                new SqlParameter("FilterCondition", ProductFilter.FilterCondition));

        }
        public override void DeleteProductFilters(int PortalId, Guid FilterSessionId)
        {
            string delCmd = "DELETE FROM " + Prefix + "ProductFilter" +
                " WHERE FiltersessionId = @FilterSessionId" +
                " AND PortalId = @PortalId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd,
                 new SqlParameter("FilterSessionid", FilterSessionId),
                 new SqlParameter("PortalId", PortalId));

        }
        public override void DeleteProductFilter(int PortalId, Guid FilterSessionId, string FilterSource)
        {
            string delCmd = "DELETE FROM " + Prefix + "ProductFilter" +
                    " WHERE FiltersessionId = @FilterSessionId" +
                    " AND PortalId = @PortalId" +
                    " AND FilterSource = @FilterSource"; 
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd,
                     new SqlParameter("FilterSessionid", FilterSessionId),
                     new SqlParameter("PortalId", PortalId),
                     new SqlParameter("FilterSource", FilterSource));
        }
        public override void DeleteProductFilter(int PortalId, Guid FilterSessionId, string FilterSource, string FirstFilterValue)
        {
            string delCmd = "DELETE FROM " + Prefix + "ProductFilter" +
                    " WHERE FiltersessionId = @FilterSessionId" +
                    " AND PortalId = @PortalId" +
                    " AND FilterSource = @FilterSource" +
                    " AND FilterValue LIKE '" + FirstFilterValue + "|%'"; 
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd,
                     new SqlParameter("FilterSessionid", FilterSessionId),
                     new SqlParameter("PortalId", PortalId),
                     new SqlParameter("FilterSource", FilterSource));
        }


        //FeatureGrid methods
        public override IDataReader GetFeatureGridValues(int PortalId, int ProductId, string Language, int RoleId, int FeatureGroupId)
        {
            string selCmd = "SELECT DISTINCT fg.FeatureGroupId,fg.ViewOrder as GViewOrder,fgl.FeatureGroup,fl.FeatureId,fl.Feature,f.FeatureToken,fl.Unit,f.FeatureListId," +
                " ISNULL((SELECT FeatureList FROM " + GetFullyQualifiedName("FeatureListLang") + " WHERE FeatureListId = f.FeatureListid AND Language = @Language),'') as FeatureList, " +
                " f.ViewOrder as FViewOrder, f.DataType,f.Control, f.MultiSelect, fv.FeatureListItemId," +
                " ISNULL((SELECT FeatureListItem FROM " + GetFullyQualifiedName("FeatureListItemLang") + " WHERE FeatureListItemId = fv.FeatureListItemid AND Language = @Language),'') as FeatureListItem, " +
                " ISNULL((SELECT Image FROM " + GetFullyQualifiedName("FeatureListItem") + " WHERE FeatureListItemId = fv.FeatureListItemid ),'') as FeatureListItemImage, " +
                " fv.nValue,fv.cValue,fv.tValue,fv.iValue,fv.fValue,fv.bValue" +
                " FROM " + GetFullyQualifiedName("Feature") + " f " +
                " INNER JOIN " + GetFullyQualifiedName("FeatureLang") + " fl ON f.FeatureId = fl.FeatureId" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureGroup") + " fg ON f.FeatureGroupId = fg.FeatureGroupId" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureGroupLang") + " fgl ON fg.FeatureGroupId = fgl.FeatureGroupId" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureValue") + " fv ON f.FeatureId = fv.FeatureId" +
                " WHERE f.PortalId = @PortalId" +
                " AND fg.PortalId = @PortalId" +
                " AND fv.ProductId = @Productid" +
                " AND fgl.Language = @Language" +
                " AND fl.Language = @Language" +
                " AND f.FeatureId IN (SELECT sf.FeatureId FROM " + GetFullyQualifiedName("Feature") + " sf " +
                "    INNER JOIN " + GetFullyQualifiedName("ProductGroupFeature") + " spgf ON sf.FeatureId = spgf.FeatureId " +
                "    INNER JOIN " + GetFullyQualifiedName("ProductInGroup") + " spg ON spgf.ProductGroupId = spg.ProductGroupId" +
                "    WHERE spg.SimpleProductId = @ProductId)" +
                " AND (fv.FeatureListitemId IS NULL OR fv.FeatureListitemId IN (" +
                "    SELECT pgli.FeatureListItemId" +
                "    FROM  " + GetFullyQualifiedName("ProductGroupListItem") + " pgli" +
                "    INNER JOIN " + GetFullyQualifiedName("ProductGroup") + " pg ON pgli.ProductGroupId = pg.ProductGroupId" +
                "    INNER JOIN " + GetFullyQualifiedName("ProductInGroup") + " pig ON pg.ProductGroupId = pig.ProductGroupId" +
                "    WHERE pig.SimpleProductId = @ProductId))";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("ProductId",ProductId),
                new SqlParameter("PortalId",PortalId),
                new SqlParameter("FeatureGroupId",FeatureGroupId),
                new SqlParameter("RoleId",RoleId),
                new SqlParameter("Language",Language)
            };

            // TODO: RoleId 

            if (FeatureGroupId > -1)
                selCmd += " AND fg.FeatureGroupId = @FeatureGroupId";

            selCmd += " ORDER BY GViewOrder, fgl.FeatureGroup, FViewOrder, fl.Feature";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetFeatureGridValueByProductAndToken(int PortalId, int ProductId, string Language, string FeatureToken )
        {
            string selCmd = "SELECT fg.FeatureGroupId,fg.ViewOrder as GViewOrder,fgl.FeatureGroup,fl.FeatureId,fl.Feature,f.FeatureToken,fl.Unit,f.FeatureListId," +
                " ISNULL((SELECT FeatureList FROM " + GetFullyQualifiedName("FeatureListLang") + " WHERE FeatureListId = f.FeatureListid AND Language = @Language),'') as FeatureList, " +
                " f.ViewOrder as FViewOrder,f.DataType,f.Control, f.MultiSelect, fv.FeatureListItemId," +
                " ISNULL((SELECT FeatureListItem FROM " + GetFullyQualifiedName("FeatureListItemLang") + " WHERE FeatureListItemId = fv.FeatureListItemid AND Language = @Language),'') as FeatureListItem, " +
                " ISNULL((SELECT Image FROM " + GetFullyQualifiedName("FeatureListItem") + " WHERE FeatureListItemId = fv.FeatureListItemid ),'') as FeatureListItemImage, " +
                " fv.nValue,fv.cValue,fv.tValue,fv.iValue,fv.fValue, fv.bValue" +
                " FROM " + GetFullyQualifiedName("Feature") + " f " +
                " INNER JOIN " + GetFullyQualifiedName("FeatureLang") + " fl ON f.FeatureId = fl.FeatureId" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureGroup") + " fg ON f.FeatureGroupId = fg.FeatureGroupId" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureGroupLang") + " fgl ON fg.FeatureGroupId = fgl.FeatureGroupId" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureValue") + " fv ON f.FeatureId = fv.FeatureId" +
                " WHERE f.PortalId = @PortalId" +
                " AND fg.PortalId = @PortalId" +
                " AND fv.ProductId = @Productid" +
                " AND fgl.Language = @Language" +
                " AND fl.Language = @Language" +
                " AND f.FeatureToken = @FeatureToken" +
                " AND (fv.FeatureListitemId IS NULL OR fv.FeatureListitemId IN (" +
                "    SELECT pgli.FeatureListItemId" +
                "    FROM  " + GetFullyQualifiedName("ProductGroupListItem") + " pgli" +
                "    INNER JOIN " + GetFullyQualifiedName("ProductGroup") + " pg ON pgli.ProductGroupId = pg.ProductGroupId" +
                "    INNER JOIN " + GetFullyQualifiedName("ProductInGroup") + " pig ON pg.ProductGroupId = pig.ProductGroupId" +
                "    WHERE pig.SimpleProductId = @ProductId))"+
                " ORDER BY GViewOrder, fgl.FeatureGroup, FViewOrder, fl.Feature";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("ProductId",ProductId),
                new SqlParameter("PortalId",PortalId),
                new SqlParameter("FeatureToken",FeatureToken),
                new SqlParameter("Language",Language)
            };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetFeatureGridFeaturesByProduct(int PortalId, int ProductId, string Language, int RoleId, int FeatureGroupId)
        {
            string selCmd = "SELECT fg.FeatureGroupId,fg.ViewOrder as GViewOrder, fgl.FeatureGroup, f.FeatureId, fl.Feature,fl.Unit," +
                " f.ViewOrder as FViewOrder,f.DataType,f.Control, f.Dimension,f.Required, f.MultiSelect, f.FeatureListId, f.MinValue," +
                " f.MaxValue,f.RegEx" +
                " FROM " + GetFullyQualifiedName("Feature") + " f " +
                " INNER JOIN " + GetFullyQualifiedName("FeatureLang") + " fl ON f.FeatureId = fl.FeatureId" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureGroup") + " fg ON f.FeatureGroupId = fg.FeatureGroupId" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureGroupLang") + " fgl ON fg.FeatureGroupId = fgl.FeatureGroupId" +
                " WHERE f.PortalId = @PortalId" +
                " AND fg.PortalId = @PortalId" +
                " AND fgl.Language = @Language" +
                " AND fl.Language = @Language" +
                " AND f.FeatureId IN (SELECT sf.FeatureId FROM " + GetFullyQualifiedName("Feature") + " sf " +
                "    INNER JOIN " + GetFullyQualifiedName("ProductGroupFeature") + " spgf ON sf.FeatureId = spgf.FeatureId " +
                "    INNER JOIN " + GetFullyQualifiedName("ProductInGroup") + " spg ON spgf.ProductGroupId = spg.ProductGroupId" +
                "    WHERE spg.SimpleProductId = @ProductId)";


            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("ProductId",ProductId),
                new SqlParameter("PortalId",PortalId),
                new SqlParameter("FeatureGroupId",FeatureGroupId),
                new SqlParameter("RoleId",RoleId),
                new SqlParameter("Language",Language)
            };

            // TODO: RoleId 

            if (FeatureGroupId > -1)
                selCmd += " AND fg.FeatureGroupId = @FeatureGroupId";

            selCmd += " ORDER BY GViewOrder, fgl.FeatureGroup, FViewOrder, fl.Feature";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetFeatureGridFeaturesByProductGroup(int PortalId, int ProductGroupId, string Language, int RoleId, int FeatureGroupId, bool OnlyShowInSearch)
        {
            string selCmd = "SELECT fg.FeatureGroupId,fg.ViewOrder as GViewOrder, fgl.FeatureGroup, f.FeatureId, fl.Feature,fl.Unit," +
                " f.ViewOrder as FViewOrder,f.DataType,f.Control, f.Dimension,f.Required, f.MultiSelect, f.FeatureListId, f.MinValue," +
                " f.MaxValue,f.RegEx,f.ViewOrder" +
                " FROM " + GetFullyQualifiedName("Feature") + " f " +
                " INNER JOIN " + GetFullyQualifiedName("FeatureLang") + " fl ON f.FeatureId = fl.FeatureId" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureGroup") + " fg ON f.FeatureGroupId = fg.FeatureGroupId" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureGroupLang") + " fgl ON fg.FeatureGroupId = fgl.FeatureGroupId" +
                " WHERE f.PortalId = @PortalId" +
                " AND fg.PortalId = @PortalId" +
                " AND fgl.Language = @Language" +
                " AND fl.Language = @Language";
            if (OnlyShowInSearch)
                selCmd += " AND f.ShowInSearch = @ShowInSearch";
            
            selCmd += " AND f.FeatureId IN (SELECT FeatureId FROM " + GetFullyQualifiedName("ProductGroupFeature") +
                "    WHERE ProductGroupId " + (ProductGroupId == -1 ? " IS NULL)" : "= @ProductGroupId)");

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("ProductGroupId",ProductGroupId),
                new SqlParameter("PortalId",PortalId),
                new SqlParameter("FeatureGroupId",FeatureGroupId),
                new SqlParameter("RoleId",RoleId),
                new SqlParameter("Language",Language),
                new SqlParameter("ShowInSearch",OnlyShowInSearch)
            };

            // TODO: RoleId 

            if (FeatureGroupId > -1)
                selCmd += " AND fg.FeatureGroupId = @FeatureGroupId";

            selCmd += " ORDER BY GViewOrder, fgl.FeatureGroup, FViewOrder, fl.Feature";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }


        // FeatureValues methods
        public override int GetFeatureValueId(int productId,int featureId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("FeatureValue") +
               " WHERE ProductId = @ProductId" +
               " AND FeatureId = @FeatureId";

            object retVal = SqlHelper.ExecuteScalar(ConnectionString,CommandType.Text,sqlCmd,
                new SqlParameter("ProductId", productId),
                new SqlParameter("FeatureId", featureId));
            return retVal != null ? (int)retVal : -1;
        }
        public override IDataReader GetFeatureValuesByPortal(int portalId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("FeatureValue") +
                            " WHERE FeatureId IN "+
                            " (SELECT FeatureId FROM " + GetFullyQualifiedName("Feature") + "  WHERE PortalId = @PortalId)";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("PortalId", portalId));
        }
        
        public override void DeleteFeatureValuesByProductId(int ProductId, int FeatureGroupId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureValue") +
               " WHERE ProductId = @ProductId";
            if (FeatureGroupId > -1)
                delCmd += " AND FeatureId IN (SELECT FeatureId FROM " + GetFullyQualifiedName("Feature") +
                    " WHERE FeatureGroupId = @FeatureGroupId)";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, 
                new SqlParameter("ProductId", ProductId),
                new SqlParameter("FeatureGroupId", FeatureGroupId));
        }
        public override int NewFeatureValue(FeatureValueInfo FeatureValue)
        {
            string DataType = "";
            if (FeatureValue.iValue != null)
                DataType = "I";
            else if (FeatureValue.nValue != null)
                DataType = "N";
            else if (FeatureValue.fValue != null)
                DataType = "F";
            else if (FeatureValue.cValue != null)
                DataType = "C";
            else if (FeatureValue.tValue != null)
                DataType = "T";
            else if (FeatureValue.bValue != null)
                DataType = "B";
            else if (FeatureValue.FeatureListItemId != null)
                DataType = "L";

            
            string insCmd = "SET NOCOUNT ON INSERT INTO " + GetFullyQualifiedName("FeatureValue");
            string fldList = " (FeatureId,ProductId,";
            string valList = " (@FeatureId,@ProductId,";
            List<SqlParameter> paraList = new List<SqlParameter>
                {
                    new SqlParameter("FeatureId", FeatureValue.FeatureId),
                    new SqlParameter("ProductId", FeatureValue.ProductId)
                };

            switch (DataType)
            {
                case "I":
                    fldList += "iValue)";
                    valList += "@iValue)";
                    paraList.Add(new SqlParameter("iValue",FeatureValue.iValue));
                    break;
                case "N":
                    fldList += "nValue)";
                    valList += "@nValue)";
                    paraList.Add( new SqlParameter("nValue",FeatureValue.nValue));
                    break;
                case "F":
                    fldList += "fValue)";
                    valList += "@fValue)";
                    paraList.Add(new SqlParameter("fValue",FeatureValue.fValue));
                    break;
                case "C":
                    fldList += "cValue)";
                    valList += "@cValue)";
                    paraList.Add(new SqlParameter("cValue",FeatureValue.cValue));
                    break;
                case "T":
                    fldList += "tValue)";
                    valList += "@tValue)";
                    paraList.Add(new SqlParameter("tValue",FeatureValue.tValue));
                    break;
                case "B":
                    fldList += "bValue)";
                    valList += "@bValue)";
                    paraList.Add(new SqlParameter("bValue", FeatureValue.bValue));
                    break;
                case "L":
                    fldList += "FeatureListItemId)";
                    valList += "@FeatureListItemId)";
                    paraList.Add(new SqlParameter("FeatureListItemId",FeatureValue.FeatureListItemId));
                    break;
                default:
                    break;
            }
            insCmd += fldList + " VALUES " + valList + " SELECT CAST(scope_identity() AS INTEGER);";
            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd, paraList.ToArray());
        }
        public override void UpdateFeatureValue(FeatureValueInfo FeatureValue)
        {
            string DataType = "";

            if (FeatureValue.iValue != null)
                DataType = "I";
            else if (FeatureValue.nValue != null)
                DataType = "N";
            else if (FeatureValue.fValue != null)
                DataType = "F";
            else if (FeatureValue.cValue != null)
                DataType = "C";
            else if (FeatureValue.tValue != null)
                DataType = "T";
            else if (FeatureValue.bValue != null)
                DataType = "B";
            else if (FeatureValue.FeatureListItemId != null)
                DataType = "L";

            string sqlCmd = "UPDATE " + GetFullyQualifiedName("FeatureValue") + " SET" +
                " FeatureId = @FeatureId," +
                " ProductId = @ProductId,";
            
            List<SqlParameter> paraList = new List<SqlParameter>
                {
                    new SqlParameter("FeatureId", FeatureValue.FeatureId),
                    new SqlParameter("ProductId", FeatureValue.ProductId),
                    new SqlParameter("FeatureValueId", FeatureValue.FeatureValueId)
                };

            switch (DataType)
            {
                case "I":
                    sqlCmd += " iValue = @iValue";
                    paraList.Add(new SqlParameter("iValue", FeatureValue.iValue));
                    break;
                case "N":
                    sqlCmd += " nValue = @nValue";
                    paraList.Add(new SqlParameter("nValue", FeatureValue.nValue));
                    break;
                case "F":
                    sqlCmd += " fValue = @fValue";
                    paraList.Add(new SqlParameter("fValue", FeatureValue.fValue));
                    break;
                case "C":
                    sqlCmd += " cValue = @cValue";
                    paraList.Add(new SqlParameter("cValue", FeatureValue.cValue));
                    break;
                case "T":
                    sqlCmd += " tValue = @tValue";
                    paraList.Add(new SqlParameter("tValue", FeatureValue.tValue));
                    break;
                case "B":
                    sqlCmd += " bValue = @bValue";
                    paraList.Add(new SqlParameter("bValue", FeatureValue.bValue));
                    break;
                case "L":
                    sqlCmd += " FeatureListItemId = @FeatureListItemId";
                    paraList.Add(new SqlParameter("FeatureListItemId", FeatureValue.FeatureListItemId));
                    break;
                default:
                    break;
            }
            sqlCmd += " WHERE FeatureValueId = @featureValueid";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, paraList.ToArray());
        }
        public override void DeleteFeatureValue(int FeatureValueId)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureValue") +
                " WHERE FeatureValueId = @FeatureValueId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("FeatureValueId", FeatureValueId));

        }

        // FeatureGroup methods
        public override IDataReader GetFeatureGroups(int PortalId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureGroup") +
                " WHERE PortalId = @PortalId" +
                " ORDER BY ViewOrder, FeatureGroupId DESC";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("PortalId", PortalId));
        }
        public override IDataReader GetFeatureGroupById(int FeatureGroupId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureGroup") +
                " WHERE FeatureGroupId = @FeatureGroupId" +
                " ORDER BY ViewOrder, FeatureGroupId DESC";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("FeatureGroupId", FeatureGroupId));
        }
        public override IDataReader GetFeatureGroupById(int FeatureGroupId, string Language)
        {
             string selCmd = "SELECT FeatureGroup.*," + 
                 " Lang.FeatureGroup" + 
                 " FROM " + GetFullyQualifiedName("FeatureGroup") + " FeatureGroup" + 
                 " INNER JOIN " + GetFullyQualifiedName("FeatureGroupLang") + " Lang" + 
                 " WHERE FeatureGroup.FeatureGroupId = @FeatureGroupId" + 
                 " AND FeatureGroupLang.Language = @Language" +
                 " ORDER BY FeatureGroup.ViewOrder, FeatureGroup.FeatureGroupId DESC";

             SqlParameter[] SqlParams = new SqlParameter[] {
                 new SqlParameter("FeatureGroupId",FeatureGroupId),
                 new SqlParameter("Language",Language)};

             return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetFeatureGroups(int PortalId, string Language)
        {
             string selCmd = "SELECT FeatureGroup.*," + 
                 " Lang.FeatureGroup" + 
                 " FROM " + GetFullyQualifiedName("FeatureGroup") + " FeatureGroup" + 
                 " INNER JOIN " + GetFullyQualifiedName("FeatureGroupLang") + " Lang" + 
                 " WHERE FeatureGroup.PortalId = @PortalId" + 
                 " AND FeatureGroupLang.Language = @Language" +
                 " ORDER BY FeatureGroup.ViewOrder, FeatureGroup.FeatureGroupId DESC";

             SqlParameter[] SqlParams = new SqlParameter[] {
                 new SqlParameter("PortalId",PortalId),
                 new SqlParameter("Language",Language)};

             return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override int NewFeatureGroup(FeatureGroupInfo FeatureGroup)
        {
            string insCmd = "SET NOCOUNT ON INSERT INTO " + GetFullyQualifiedName("FeatureGroup") +
                " (PortalID,ViewOrder)" +
                " VALUES " +
                " (@PortalID,@ViewOrder) SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter[] SqlParams = new SqlParameter[]
                                        {
                                            new SqlParameter("PortalID", FeatureGroup.PortalID),
                                            new SqlParameter("ViewOrder", FeatureGroup.ViewOrder)
                                        };

            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateFeatureGroup(FeatureGroupInfo FeatureGroup)
        {
            string updCmd = "UPDATE " + GetFullyQualifiedName("FeatureGroup") + " SET " +
                " PortalID = @PortalID," +
                " ViewOrder = @ViewOrder" +
                " WHERE FeatureGroupId = @FeatureGroupId";

            SqlParameter[] SqlParams = new SqlParameter[]
                                        {
                                            new SqlParameter("FeatureGroupId", FeatureGroup.FeatureGroupId),
                                            new SqlParameter("PortalID", FeatureGroup.PortalID),
                                            new SqlParameter("ViewOrder", FeatureGroup.ViewOrder)
                                        };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteFeatureGroup(int FeatureGroupId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureGroup") +
                " WHERE FeatureGroupId = @FeatureGroupId;";
             SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("FeatureGroupId",FeatureGroupId));
        }
        public override void DeleteFeatureGroups(int PortalId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureGroup") +
                " WHERE PortalId = @PortalId;";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("PortalId", PortalId));
        }

        // FeatureGroupLang methods
        public override IDataReader GetFeatureGroupLangs(int FeatureGroupId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureGroupLang") +
                " WHERE FeatureGroupId = @FeatureGroupId";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("FeatureGroupId", FeatureGroupId));
        }
        public override IDataReader GetFeatureGroupLangsByPortal(int portalId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureGroupLang") +
                " WHERE FeatureGroupId IN (SELECT FeatureGroupId FROM " + GetFullyQualifiedName("FeatureGroup") +  " WHERE PortalId = @PortalId)";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("PortalId", portalId));
        }

        
        public override IDataReader GetFeatureGroupLang(int FeatureGroupId, string Language)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureGroupLang") +
                " WHERE FeatureGroupId = @FeatureGroupId" +
                " And Language = @Language";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureGroupId", FeatureGroupId),
                new	SqlParameter("Language",Language)};
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override void NewFeatureGroupLang(FeatureGroupLangInfo FeatureGroupLang)
        {
            string insCmd = "INSERT INTO " + GetFullyQualifiedName("FeatureGroupLang") +
                " (FeatureGroupId,Language,FeatureGroup)" +
                " VALUES " +
                " (@FeatureGroupId,@Language,@FeatureGroup)";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureGroupId",FeatureGroupLang.FeatureGroupId),
                new SqlParameter("Language",FeatureGroupLang.Language),
                new SqlParameter("FeatureGroup",FeatureGroupLang.FeatureGroup)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateFeatureGroupLang(FeatureGroupLangInfo FeatureGroupLang)
        {
            string updCmd = "UPDATE " + GetFullyQualifiedName("FeatureGroupLang") + " SET " +
                " FeatureGroup = @FeatureGroup" +
                " WHERE Language = @Language" +
                " AND FeatureGroupId = @FeatureGroupId";

             SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureGroupId",FeatureGroupLang.FeatureGroupId),
                new SqlParameter("Language",FeatureGroupLang.Language),
                new SqlParameter("FeatureGroup",FeatureGroupLang.FeatureGroup)};

             SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteFeatureGroupLangs(int FeatureGroupId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureGroupLang") +
                " WHERE FeatureGroupId = @FeatureGroupId;";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("FeatureGroupId", FeatureGroupId));
        }
        public override void DeleteFeatureGroupLang(int FeatureGroupId, string Language)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureGroupLang") +
                " WHERE FeatureGroupId = @FeatureGroupId" +
                " AND Language = @Language";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureGroupId",FeatureGroupId),
                new SqlParameter("Language",Language)};
            
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
        }

        // Feature methods
        public override IDataReader GetFeatures(int PortalId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("Feature") +
                " WHERE PortalId = @PortalId" +
                " ORDER BY ViewOrder,FeatureId DESC";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("PortalId", PortalId));
        }
        public override IDataReader GetFeatures(int PortalId, string Language)
        {
            string selCmd = "SELECT Feature.*," +
                " Lang.Feature,Lang.Unit" +
                " FROM " + GetFullyQualifiedName("Feature") + " Feature" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureLang") + " Lang" +
                " WHERE Feature.PortalId = @PortalId" +
                " AND FeatureLang.Language = @Language" +
                " ORDER BY Feature.ViewOrder,Feature.FeatureId DESC";

            SqlParameter[] SqlParams = new SqlParameter[] {
         new SqlParameter("PortalId",PortalId),
         new SqlParameter("Language",Language)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetFeatureById(int FeatureId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("Feature") +
                " WHERE FeatureId = @FeatureId" +
                " ORDER BY ViewOrder,FeatureId DESC";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("FeatureId", FeatureId));
        }
        public override IDataReader GetFeatureById(int FeatureId, string Language)
        {
            string selCmd = "SELECT Feature.*," +
                " Lang.Feature,Lang.Unit" +
                " FROM " + GetFullyQualifiedName("Feature") + " Feature" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureLang") + " Lang" +
                " WHERE Feature.FeatureId = @FeatureId" +
                " AND Lang.Language = @Language" +
                " ORDER BY Feature.ViewOrder,Feature.FeatureId DESC";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureId",FeatureId),
                new SqlParameter("Language",Language)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override int NewFeature(FeatureInfo Feature)
        {
            string insCmd = "SET NOCOUNT ON INSERT INTO " + GetFullyQualifiedName("Feature") +
                " (PortalID,FeatureGroupId,FeatureListId,Datatype,Multiselect,Control,"+
                "  Dimension,Required,MinValue,MaxValue,RegEx,RoleID,ShowInSearch,SearchGroups,FeatureToken,ViewOrder)" +
                " VALUES " +
                " (@PortalID,@FeatureGroupId,@FeatureListId,@Datatype,@Multiselect,@Control,"+
                " @Dimension,@Required,@MinValue,@MaxValue,@RegEx,@RoleID,@ShowInSearch,@SearchGroups,@FeatureToken,@ViewOrder)"+
                " SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureId",Feature.FeatureId),
                new SqlParameter("PortalID",Feature.PortalID),
                new SqlParameter("FeatureGroupId",Feature.FeatureGroupId),
                new SqlParameter("FeatureListId",GetNull(Feature.FeatureListId)),
                new SqlParameter("Datatype",Feature.Datatype),
                new SqlParameter("Multiselect",Feature.Multiselect),
                new SqlParameter("Control",Feature.Control),
                new SqlParameter("Dimension",Feature.Dimension),
                new SqlParameter("Required",Feature.Required),
                new SqlParameter("MinValue",Feature.MinValue),
                new SqlParameter("MaxValue",Feature.MaxValue),
                new SqlParameter("RegEx",Feature.RegEx),
                new SqlParameter("RoleID",Feature.RoleID),
                new SqlParameter("ShowInSearch",Feature.ShowInSearch),
                new SqlParameter("SearchGroups",Feature.SearchGroups),
                new SqlParameter("FeatureToken",Feature.FeatureToken),
                new SqlParameter("ViewOrder",Feature.ViewOrder),
            };

            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateFeature(FeatureInfo Feature)
        {
            string updCmd = "UPDATE " + GetFullyQualifiedName("Feature") + " SET " +
                " PortalID = @PortalID," +
                " FeatureGroupId = @FeatureGroupId," +
                " FeatureListId = @FeatureListId," +
                " Datatype = @Datatype," +
                " Multiselect = @Multiselect," +
                " Control = @Control," +
                " Dimension = @Dimension," +
                " Required = @Required," +
                " MinValue = @MinValue," +
                " MaxValue = @MaxValue," +
                " RegEx = @RegEx," +
                " RoleID = @RoleID," +
                " ShowInSearch = @ShowInSearch," +
                " SearchGroups = @SearchGroups," +
                " FeatureToken = @FeatureToken," +
                " ViewOrder = @ViewOrder" +
                " WHERE FeatureId = @FeatureId";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureId",Feature.FeatureId),
                new SqlParameter("PortalID",Feature.PortalID),
                new SqlParameter("FeatureGroupId",Feature.FeatureGroupId),
                new SqlParameter("FeatureListId",GetNull(Feature.FeatureListId)),
                new SqlParameter("Datatype",Feature.Datatype),
                new SqlParameter("Multiselect",Feature.Multiselect),
                new SqlParameter("Control",Feature.Control),
                new SqlParameter("Dimension",Feature.Dimension),
                new SqlParameter("Required",Feature.Required),
                new SqlParameter("MinValue",Feature.MinValue),
                new SqlParameter("MaxValue",Feature.MaxValue),
                new SqlParameter("RegEx",Feature.RegEx),
                new SqlParameter("RoleID",Feature.RoleID),
                new SqlParameter("ShowInSearch",Feature.ShowInSearch),
                new SqlParameter("SearchGroups",Feature.SearchGroups),
                new SqlParameter("FeatureToken",Feature.FeatureToken),
                new SqlParameter("ViewOrder",Feature.ViewOrder)
            };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteFeature(int FeatureId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("Feature") +
                " WHERE FeatureId = @FeatureId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("FeatureId", FeatureId));
        }
        public override void DeleteFeatures(int PortalId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("Feature") +
                " WHERE PortalId = @PortalId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("PortalId", PortalId));
        }

        // FeatureLang methods
        public override IDataReader GetFeatureLangs(int FeatureId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureLang") +
                " WHERE FeatureId = @FeatureId";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("FeatureId", FeatureId));
        }
        public override IDataReader GetFeatureLangsByPortal(int portalId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureLang") +
                " WHERE FeatureId IN (SELECT FeatureId FROM " + GetFullyQualifiedName("Feature") +  " WHERE PortalId = @PortalId)";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("PortalId", portalId));
        }
        public override IDataReader GetFeatureLang(int FeatureId, string Language)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureLang") +
                " WHERE FeatureId = @FeatureId" +
                " And Language = @Language";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureId", FeatureId),
                new	SqlParameter("Language",Language)};
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override void NewFeatureLang(FeatureLangInfo FeatureLang)
        {
            string insCmd = "INSERT INTO " + GetFullyQualifiedName("FeatureLang") +
                " (FeatureId,Language,Feature,Unit)" +
                " VALUES " +
                " (@FeatureId,@Language,@Feature,@Unit)";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureId",FeatureLang.FeatureId),
                new SqlParameter("Language",FeatureLang.Language.Trim()),
                new SqlParameter("Feature",FeatureLang.Feature),
                new SqlParameter("Unit",FeatureLang.Unit)};

            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd, SqlParams);
            }
            catch (Exception ex)
            {
                throw new SqlExecutionException(String.Format("Language:'{0}'  Feature: '{1}'  Unit '{2}' : {3}",FeatureLang.Language,FeatureLang.Feature,FeatureLang.Unit,ex));
            }
            
        }
        public override void UpdateFeatureLang(FeatureLangInfo FeatureLang)
        {
            string updCmd = "UPDATE " + GetFullyQualifiedName("FeatureLang") + " SET " +
                " Feature = @Feature" +
                " WHERE Language = @Language" +
                " AND FeatureId = @FeatureId";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureId",FeatureLang.FeatureId),
                new SqlParameter("Language",FeatureLang.Language.Trim()),
                new SqlParameter("Feature",FeatureLang.Feature)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteFeatureLangs(int FeatureId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureLang") +
                " WHERE FeatureId = @FeatureId;";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("FeatureId", FeatureId));
        }
        public override void DeleteFeatureLang(int FeatureId, string Language)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureLang") +
                " WHERE FeatureId = @FeatureId" +
                " AND Language = @Language";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureId",FeatureId),
                new SqlParameter("Language",Language)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
        }
        
        // FeatureList methods
        public override IDataReader GetFeatureLists(int PortalId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureList") +
                " WHERE PortalId = @PortalId" +
                " ORDER BY FeatureListId ASC";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("PortalId", PortalId));
        }
        public override IDataReader GetFeatureLists(int PortalId, string Language)
        {
            string selCmd = "SELECT FeatureList.*," +
                " Lang.FeatureList" +
                " FROM " + GetFullyQualifiedName("FeatureList") + " FeatureList" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureListLang") + " Lang ON FeatureList.FeatureListId = Lang.FeatureListId" + 
                " WHERE FeatureList.PortalId = @PortalId" +
                " AND Lang.Language = @Language" +
                " ORDER BY Lang.FeatureList,FeatureList.FeatureListId ASC";

            SqlParameter[] SqlParams = new SqlParameter[] {
         new SqlParameter("PortalId",PortalId),
         new SqlParameter("Language",Language)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetFeatureListById(int FeatureListId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureList") +
                " WHERE FeatureListId = @FeatureListId" +
                " ORDER BY FeatureListId DESC";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("FeatureListId", FeatureListId));
        }
        public override IDataReader GetFeatureListById(int FeatureListId, string Language)
        {
            string selCmd = "SELECT FeatureList.*," +
                " Lang.FeatureList" +
                " FROM " + GetFullyQualifiedName("FeatureList") + " FeatureList" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureListLang") + " Lang ON FeatureList.FeatureListId = Lang.FeatureListId" +
                " WHERE FeatureList.FeatureListId = @FeatureListId" +
                " AND Lang.Language = @Language" +
                " ORDER BY FeatureList.FeatureListId DESC";

            SqlParameter[] SqlParams = new SqlParameter[] {
         new SqlParameter("FeatureListId",FeatureListId),
         new SqlParameter("Language",Language)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override int NewFeatureList(FeatureListInfo FeatureList)
        {
            string insCmd = "SET NOCOUNT ON INSERT INTO " + GetFullyQualifiedName("FeatureList") +
                " (PortalID)" +
                " VALUES " +
                " (@PortalID) SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("PortalID",FeatureList.PortalID)};

            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateFeatureList(FeatureListInfo FeatureList)
        {
            string updCmd = "UPDATE " + GetFullyQualifiedName("FeatureList") + " SET " +
                " PortalID = @PortalID" +
                " WHERE FeatureListId = @FeatureListId";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureListId",FeatureList.FeatureListId),
                new SqlParameter("PortalID",FeatureList.PortalID)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteFeatureList(int FeatureListId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureList") +
                " WHERE FeatureListId = @FeatureListId";
             SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("FeatureListId",FeatureListId));
        }
        public override void DeleteFeatureLists(int PortalId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureList") +
                " WHERE PortalId = @PortalId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("PortalId", PortalId));
        }


        // FeatureListLang methods
        public override IDataReader GetFeatureListLangs(int FeatureListId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureListLang") +
                " WHERE FeatureListId = @FeatureListId";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("FeatureListId", FeatureListId));
        }
        public override IDataReader GetFeatureListLangsByPortal(int portalId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureListLang") +
                " WHERE FeatureListId IN (SELECT FeatureListId FROM " + GetFullyQualifiedName("FeatureList") + " WHERE PortalId = @PortalId)";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("PortalId", portalId));
        }
        public override IDataReader GetFeatureListLang(int FeatureListId, string Language)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureListLang") +
                " WHERE FeatureListId = @FeatureListId" +
                " And Language = @Language";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureListId", FeatureListId),
                new	SqlParameter("Language",Language)};
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override void NewFeatureListLang(FeatureListLangInfo FeatureListLang)
        {
            string insCmd = "INSERT INTO " + GetFullyQualifiedName("FeatureListLang") +
                " (FeatureListId,Language,FeatureList)" +
                " VALUES " +
                " (@FeatureListId,@Language,@FeatureList)";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureListId",FeatureListLang.FeatureListId),
                new SqlParameter("Language",FeatureListLang.Language),
                new SqlParameter("FeatureList",FeatureListLang.FeatureList)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateFeatureListLang(FeatureListLangInfo FeatureListLang)
        {
            string updCmd = "UPDATE " + GetFullyQualifiedName("FeatureListLang") + " SET " +
                " FeatureList = @FeatureList" +
                " WHERE Language = @Language" +
                " AND FeatureListId = @FeatureListId";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureListId",FeatureListLang.FeatureListId),
                new SqlParameter("Language",FeatureListLang.Language),
                new SqlParameter("FeatureList",FeatureListLang.FeatureList)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteFeatureListLangs(int FeatureListId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureListLang") +
                " WHERE FeatureListId = @FeatureListId;";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("FeatureListId", FeatureListId));
        }
        public override void DeleteFeatureListLang(int FeatureListId, string Language)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureListLang") +
                " WHERE FeatureListId = @FeatureListId" +
                " AND Language = @Language";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureListId",FeatureListId),
                new SqlParameter("Language",Language)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
        }

        // FeatureListItem methods
        public override IDataReader GetFeatureListItemById(int FeatureListItemId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureListItem") +
                " WHERE FeatureListItemId = @FeatureListItemId" +
                " ORDER BY FeatureListItemId DESC";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("FeatureListItemId", FeatureListItemId));
        }
        public override IDataReader GetFeatureListItemById(int FeatureListItemId, string Language)
        {
            string selCmd = "SELECT FeatureListItem.*," +
                " Lang.FeatureListItem" +
                " FROM " + GetFullyQualifiedName("FeatureListItem") + " FeatureListItem" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureListItemLang") + " Lang ON FeatureListItem.FeatureListItemId = Lang.FeatureListItemId" +
                " WHERE FeatureListItem.FeatureListItemId = @FeatureListItemId" +
                " AND Lang.Language = @Language";

            SqlParameter[] SqlParams = new SqlParameter[] {
                 new SqlParameter("FeatureListItemId",FeatureListItemId),
                 new SqlParameter("Language",Language)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetFeatureListItems(int portalId)
        {
            string selCmd = "SELECT fli.* FROM " + GetFullyQualifiedName("FeatureListItem") + " fli" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureList") +  " fl ON fl.FeatureListid = fli.featureListId" +
                " WHERE fl.Portalid = @PortalId";

            SqlParameter[] SqlParams = new SqlParameter[] {
                 new SqlParameter("PortalId",portalId)
            };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }

        public override IDataReader GetFeatureListItemsByListId(int FeatureListId)
        {
            string selCmd = "SELECT fli.*, '' as FeatureListItem" +
                " FROM " + GetFullyQualifiedName("FeatureListItem") + " fli" +
                " WHERE fli.FeatureListId = @FeatureListId" +
                " ORDER BY fli.ViewOrder, fli.FeatureListItemId ASC";

            SqlParameter[] SqlParams = new SqlParameter[] {
                 new SqlParameter("FeatureListId",FeatureListId)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetFeatureListItemsByListId(int FeatureListId, string Language, bool onlyWithImage)
        {
            string selCmd = "SELECT fli.*," +
                " flil.FeatureListItem" +
                " FROM " + GetFullyQualifiedName("FeatureListItem") + " fli" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureListItemLang") + " flil ON fli.FeatureListItemId = flil.FeatureListItemId" +
                " WHERE fli.FeatureListId = @FeatureListId" +
                (onlyWithImage ? " AND fli.Image <> ''" : "") +
                " AND flil.Language = @Language" +
                " ORDER BY fli.ViewOrder, flil.FeatureListItem ASC";

            SqlParameter[] SqlParams = new SqlParameter[] {
                 new SqlParameter("FeatureListId",FeatureListId),
                 new SqlParameter("Language",Language)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetFeatureListItemsByListAndProduct(int FeatureListId, int ProductId, string Language)
        {
            string selCmd = "SELECT fli.*," +
                " flil.FeatureListItem" +
                " FROM " + GetFullyQualifiedName("FeatureListItem") + " fli" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureListItemLang") + " flil ON fli.FeatureListItemId = flil.FeatureListItemId" +
                " WHERE fli.FeatureListId = @FeatureListId" +
                " AND flil.Language = @Language" +
                " AND fli.FeatureListItemId IN (" +
                "    SELECT pgli.FeatureListItemId" +
                "    FROM  " + GetFullyQualifiedName("ProductGroupListItem") + " pgli" +
                "    INNER JOIN " + GetFullyQualifiedName("ProductGroup") + " pg ON pgli.ProductGroupId = pg.ProductGroupId" +
                "    INNER JOIN " + GetFullyQualifiedName("ProductInGroup") + " pig ON pg.ProductGroupId = pig.ProductGroupId" +
                "    WHERE pig.SimpleProductId = @ProductId)" +
                " ORDER BY fli.ViewOrder,flil.FeatureListItem ASC";

            SqlParameter[] SqlParams = new SqlParameter[] {
                 new SqlParameter("FeatureListId",FeatureListId),
                 new SqlParameter("ProductId",ProductId),
                 new SqlParameter("Language",Language)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetFeatureListItemsByListAndProductGroup(int FeatureListId, int ProductGroupId, string Language)
        {
            string selCmd = "SELECT fli.*," +
                " flil.FeatureListItem" +
                " FROM " + GetFullyQualifiedName("FeatureListItem") + " fli" +
                " INNER JOIN " + GetFullyQualifiedName("FeatureListItemLang") + " flil ON fli.FeatureListItemId = flil.FeatureListItemId" +
                " WHERE fli.FeatureListId = @FeatureListId" +
                " AND flil.Language = @Language" +
                " AND fli.FeatureListItemId IN (" +
                "    SELECT pgli.FeatureListItemId" +
                "    FROM  " + GetFullyQualifiedName("ProductGroupListItem") + " pgli" +
                "    INNER JOIN " + GetFullyQualifiedName("ProductGroup") + " pg ON pgli.ProductGroupId = pg.ProductGroupId" +
                "    WHERE pg.ProductGroupId = @ProductGroupId)" +
                " ORDER BY fli.ViewOrder,flil.FeatureListItem ASC";

            SqlParameter[] SqlParams = new SqlParameter[] {
                 new SqlParameter("FeatureListId",FeatureListId),
                 new SqlParameter("ProductGroupId",ProductGroupId),
                 new SqlParameter("Language",Language)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override int NewFeatureListItem(FeatureListItemInfo FeatureListItem)
        {
            string insCmd = "SET NOCOUNT ON INSERT INTO " + GetFullyQualifiedName("FeatureListItem") +
                " (FeatureListId,Image,ViewOrder)" +
                " VALUES " +
                " (@FeatureListId,@Image,@ViewOrder) SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureListId",FeatureListItem.FeatureListId),
                new SqlParameter("Image",FeatureListItem.Image),
                new SqlParameter("ViewOrder",FeatureListItem.ViewOrder)
            };

            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateFeatureListItem(FeatureListItemInfo FeatureListItem)
        {
            string updCmd = "UPDATE " + GetFullyQualifiedName("FeatureListItem") + " SET " +
                " FeatureListId = @FeatureListId," +
                " Image = @Image," +
                " ViewOrder = @ViewOrder" +
                " WHERE FeatureListItemId = @FeatureListItemId";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureListItemId",FeatureListItem.FeatureListItemId),
                new SqlParameter("FeatureListId",FeatureListItem.FeatureListId),
                new SqlParameter("Image",FeatureListItem.Image),
                new SqlParameter("ViewOrder",FeatureListItem.ViewOrder)
            };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteFeatureListItem(int FeatureListItemId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureListItem") +
                " WHERE FeatureListItemId = @FeatureListItemId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("FeatureListItemId", FeatureListItemId));
        }
        public override void DeleteFeatureListItems(int FeatureListId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureListItem") +
                " WHERE FeatureListId = @FeatureListId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("FeatureListId", FeatureListId));
        }

        // FeatureListItemLang methods
        public override IDataReader GetFeatureListItemLangs(int FeatureListItemId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureListItemLang") +
                " WHERE FeatureListItemId = @FeatureListItemId";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("FeatureListItemId", FeatureListItemId));
        }
        public override IDataReader GetFeatureListItemLangsByPortal(int portalId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureListItemLang") +
                " WHERE FeatureListItemId IN " +
                " (SELECT FeatureListItemId FROM " + GetFullyQualifiedName("FeatureListItem") + " fli " +
                "  INNER JOIN " + GetFullyQualifiedName("FeatureList") +  " fl ON fli.FeatureListId = fl.FeatureListId" +
                "  WHERE fl.PortalId = @PortalId)";

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("PortalId", portalId));
        }

        public override IDataReader GetFeatureListItemLang(int FeatureListItemId, string Language)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("FeatureListItemLang") +
                " WHERE FeatureListItemId = @FeatureListItemId" +
                " And Language = @Language";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureListItemId", FeatureListItemId),
                new	SqlParameter("Language",Language)};
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override void NewFeatureListItemLang(FeatureListItemLangInfo FeatureListItemLang)
        {
            string insCmd = "SET NOCOUNT ON INSERT INTO " + GetFullyQualifiedName("FeatureListItemLang") +
                " (FeatureListItemId,Language,FeatureListItem)" +
                " VALUES " +
                " (@FeatureListItemId,@Language,@FeatureListItem) SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureListItemId",FeatureListItemLang.FeatureListItemId),
                new SqlParameter("Language",FeatureListItemLang.Language),
                new SqlParameter("FeatureListItem",FeatureListItemLang.FeatureListItem)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateFeatureListItemLang(FeatureListItemLangInfo FeatureListItemLang)
        {
            string updCmd = "UPDATE " + GetFullyQualifiedName("FeatureListItemLang") + " SET " +
                " FeatureListItem = @FeatureListItem" +
                " WHERE Language = @Language" +
                " AND FeatureListItemId = @FeatureListItemId";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureListItemId",FeatureListItemLang.FeatureListItemId),
                new SqlParameter("Language",FeatureListItemLang.Language),
                new SqlParameter("FeatureListItem",FeatureListItemLang.FeatureListItem)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteFeatureListItemLangs(int FeatureListItemId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureListItemLang") +
                " WHERE FeatureListItemId = @FeatureListItemId;";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("FeatureListItemId", FeatureListItemId));
        }
        public override void DeleteFeatureListItemLang(int FeatureListItemId, string Language)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("FeatureListItemLang") +
                " WHERE FeatureListItemId = @FeatureListItemId" +
                " AND Language = @Language";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("FeatureListItemId",FeatureListItemId),
                new SqlParameter("Language",Language)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
        }

        // ProductGroupFeature methods
        public override DataTable GetProductGroupFeatures(int featureId)
        {
            string selCmd = "SELECT ProductGroupId FROM " + GetFullyQualifiedName("ProductGroupFeature") + 
                " WHERE FeatureId = " + featureId.ToString();
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, selCmd);
            return ds.Tables[0];
        }
        public override IDataReader GetProductGroupFeaturesByPortal(int portalid)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("ProductGroupFeature") +
                " WHERE ProductGroupId IN (SELECT ProductGroupId FROM " + GetFullyQualifiedName("ProductGroup") + " WHERE PortalId = @PortalId)" ;
            return (IDataReader) SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("PortalId",portalid));
        }

        public override bool IsFeatureInProductGroup(int productGroupId, int featureId)
        {
            string selCmd = "SELECT ProductGroupId FROM " + GetFullyQualifiedName("ProductGroupFeature") +
                            " WHERE FeatureId = @FeatureId" +
                            " AND ProductGroupId = @ProductGroupId";
            
            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("FeatureId",featureId),
                new SqlParameter("ProductGroupId",productGroupId)};

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, selCmd, sqlParams);
            return ds.Tables[0].Rows.Count > 0;
        }

        public override void NewProductGroupFeature(int FeatureId, int ProductGroupId)
        {
            string insCmd = "INSERT INTO " + Prefix + "ProductGroupFeature (FeatureId,ProductGroupId) VALUES (" +
                FeatureId.ToString() + "," +
                ProductGroupId.ToString() + ")";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd);

        }
        public override void DeleteProductGroupFeatures(int FeatureId)
        {
            string delCmd = "DELETE FROM " + Prefix + "ProductGroupFeature WHERE FeatureId = " + FeatureId.ToString();
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd);
        }
        public override void DeleteProductGroupFeature(int FeatureId, int ProductGroupId)
        {
            string delCmd = "DELETE FROM " + Prefix + "ProductGroupFeature " +
                " WHERE FeatureId = @FeatureId AND ProductGroupId = @ProductGroupId";

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("FeatureId",FeatureId),
                new SqlParameter("ProductGroupId",ProductGroupId)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, param);
        }

        // ProductGroupListItems methods
        public override IDataReader GetProductGroupListItemsByPortal(int portalId)
        {
            string sqlCmd = " SELECT * FROM " + GetFullyQualifiedName("ProductGroupListItem") +
                " WHERE ProductGroupId IN " +
                " (SELECT ProductGroupId FROM " + GetFullyQualifiedName("ProductGroup") + " WHERE PortalId = @PortalId)";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("PortalId", portalId));
        }
        public override void NewProductGroupListItem(int productGroupId, int featureListItemId)
        {
            string insCmd = "INSERT INTO " + Prefix + "ProductGroupListItem (FeatureListItemId,ProductGroupId) VALUES (" +
                featureListItemId.ToString() + "," +
                productGroupId.ToString() + ")";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd);
        }
        public override bool IsFeatureListItemInProductGroup(int productGroupId, int featureListItemid)
        {
            {
                string selCmd = "SELECT ProductGroupId FROM " + GetFullyQualifiedName("ProductGroupListItem") +
                                " WHERE FeatureListItemId = @FeatureListItemId" +
                                " AND ProductGroupId = @ProductGroupId";

                SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("FeatureListItemId",featureListItemid),
                new SqlParameter("ProductGroupId",productGroupId)};

                DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, selCmd, sqlParams);
                return ds.Tables[0].Rows.Count > 0;
            }
        }
        public override void DeleteProductGroupListItem(int productGroupId, int featureListItemId)
        {
            string delCmd = "DELETE FROM " + Prefix + "ProductGroupListItem " +
                " WHERE FeatureListItemId = @FeatureListItemId AND ProductGroupId = @ProductGroupId";

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("FeatureListItemId",featureListItemId),
                new SqlParameter("ProductGroupId",productGroupId)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, param);
        }

        public override void DeleteProductGroupListItemsByPortal(int portalId)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("ProductGroupListItem") +
                            " WHERE ProductGroupId IN" +
                            " (SELECT ProductGroupId FROM " + GetFullyQualifiedName("ProductGroup") +
                            " WHERE PortalId = @PortalId)";

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("PortalId", portalId));
        }

        public override void DeleteProductGroupListItemsByProductGroup(int productGroupId)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("ProductGroupListItem") +
                            " WHERE ProductGroupId = @ProductGroupId" ;

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("ProductGroupId", productGroupId));
        }

        public override void DeleteProductGroupListItemsByProductGroupAndFeatureList(int productgroupId,int featureListId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("ProductGroupListItem") +
                            " WHERE ProductGroupId = @ProductGroupId" +
                            " AND FeatureListItemId IN " +
                            " (SELECT FeatureListItemId FROM " + GetFullyQualifiedName("FeatureListItem") +
                            "  WHERE FeatureListId = @FeatureListId)";

            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("ProductGroupId", productgroupId),
                    new SqlParameter("FeatureListId", featureListId)
                };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, sqlParams);
        }

        public override void AddProductGroupListItemsByProductGroupAndFeatureList(int productgroupId, int featureListId)
        {
            string delCmd = "INSERT INTO " + GetFullyQualifiedName("ProductGroupListItem") + " (ProductGroupId,FeatureListItemId) " +
                            " SELECT " + productgroupId.ToString() +  " AS ProductGroupId, FeatureListItemId " +
                            " FROM " + GetFullyQualifiedName("FeatureListItem") +
                            " WHERE FeatureListId = @FeatureListId";

            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("FeatureListId", featureListId)
                };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, sqlParams);
        }

        public override IDataReader GetSelectedFeatureListsByProductGroup(int productGroupId, string language)
        {
            string sqlCmd = "SELECT DISTINCT fl.*,fll.FeatureList FROM " + GetFullyQualifiedName("FeatureList") + " fl" +
                            " INNER JOIN " + GetFullyQualifiedName("FeatureListLang") + " fll ON fl.FeatureListId = fll.FeatureListId" +
                            " INNER JOIN " + GetFullyQualifiedName("FeatureListItem") + " fli ON fl.FeatureListId = fli.FeatureListId" +
                            " INNER JOIN " + GetFullyQualifiedName("ProductGroupListItem") + " pgfl ON fli.FeatureListItemId = pgfl.FeatureListItemId" +
                            " WHERE pgfl.ProductGroupId = @ProductGroupId" +
                            " AND fll.Language = @language";

            SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("ProductGroupId", productGroupId),
                    new SqlParameter("Language", language), 
                };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);

        }
        // StaticFilter methods
        public override IDataReader GetStaticFilters(int PortalId)
        {
            string selCmd = "SELECT * FROM " + GetFullyQualifiedName("StaticFilter") + " WHERE PortalId = " + PortalId.ToString();
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override IDataReader GetStaticFilter(int PortalId, string Token)
        {
            string selCmd = "SELECT * FROM " + GetFullyQualifiedName("StaticFilter") +
                " WHERE PortalId = " + PortalId.ToString() +
                " AND Token = @Token";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("Token",Token));
        }
        public override IDataReader GetStaticFilterById(int StaticFilterId)
        {
            string selCmd = "SELECT * FROM " + GetFullyQualifiedName("StaticFilter") +
                " WHERE StaticFilterId = " + StaticFilterId.ToString();
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
        }
        public override int NewStaticFilter(StaticFilterInfo StaticFilter)
        {
            string insCmd = "SET NOCOUNT ON INSERT INTO " + GetFullyQualifiedName("StaticFilter") +
               " (PortalId,Token,FilterCondition)" +
               " VALUES " +
               " (@PortalId,@Token,@FilterCondition) SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter[] SqlParams = new SqlParameter[] {
               new SqlParameter("PortalId",StaticFilter.PortalId),
               new SqlParameter("Token",StaticFilter.Token),
               new SqlParameter("FilterCondition",StaticFilter.FilterCondition) };

            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateStaticFilter(StaticFilterInfo StaticFilter)
        {
            string updCmd = "UPDATE " + GetFullyQualifiedName("StaticFilter") +" SET " +
                " FilterCondition = @FilterCondition," +
                " PortalId = @PortalId," +
                " Token = @Token" + 
                " WHERE StaticFilterId = @StaticFilterId";

            SqlParameter[] SqlParams = new SqlParameter[] {
               new SqlParameter("PortalId",StaticFilter.PortalId),
               new SqlParameter("Token",StaticFilter.Token),
               new SqlParameter("FilterCondition",StaticFilter.FilterCondition),
               new SqlParameter("StaticFilterid", StaticFilter.StaticFilterId) };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteStaticFilter(int PortalId, string Token)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("StaticFilter") +
                " WHERE PortalId = @PortalId" +
                " AND Token = @Token";

            SqlParameter[] SqlParams = new SqlParameter[] {
               new SqlParameter("PortalId",PortalId),
               new SqlParameter("Token",Token) };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
        }
        public override void DeleteStaticFilterById(int StaticFilterId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("StaticFilter") +
                " WHERE StaticFilterId = @StaticFilterId";

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("StaticFilterId",StaticFilterId));
        }


        // LocalResources methods
        public override IDataReader GetLocalResource(int portalId, string token)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("LocalResource") +
                            " WHERE PortalId = @PortalId AND LocalResourceToken = @LocalResourceToken";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("PortalId",portalId), 
                                               new SqlParameter("LocalResourceToken",token), 
                                           };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        public override int NewLocalResource(LocalResourceInfo resourceInfo)
        {
            string sqlCmd = "SET NOCOUNT ON INSERT INTO " + GetFullyQualifiedName("LocalResource") +
                            " (PortalId,LocalResourceToken) VALUES (@PortalId,@LocalResourceToken) " +
                            " SELECT CAST(scope_identity() AS INTEGER);";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("PortalId",resourceInfo.PortalId), 
                                               new SqlParameter("LocalResourceToken",resourceInfo.LocalResourceToken), 
                                           };
            return (int) SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override void UpdateLocalResource(LocalResourceInfo resourceInfo)
        {
            string sqlCmd = "UPDATE " + GetFullyQualifiedName("LocalResource") + " SET " +
                            " PortalId = @PortalId, " +
                            " LocalResourceToken = @LocalResourceToken " +
                            " WHERE LocalResourceId = @LocalResourceId";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("PortalId",resourceInfo.PortalId), 
                                               new SqlParameter("LocalResourceToken",resourceInfo.LocalResourceToken), 
                                               new SqlParameter("LocalResourceId",resourceInfo.LocalResourceId), 
                                           };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);

        }
        public override void DeleteLocalResource(int resourceId)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("LocalResource") + 
                            " WHERE LocalResourceId = @LocalResourceId";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("LocalResourceId",resourceId), 
                                           };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        //LocalResourceLang methods
        public override IDataReader GetLocalResourceLang(int resourceId, string language)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("LocalResourceLang") + 
                " WHERE LocalResourceId = @LocalResourceId AND Language = @Language";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("LocalResourceId",resourceId), 
                                               new SqlParameter("Language", language), 
                                           };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);

        }
        public override IDataReader GetLocalResourceLang(int portalId, string token, string language)
        {

            string sqlCmd = "SELECT rl.* FROM " + GetFullyQualifiedName("LocalResource") + " r " +
                " INNER JOIN " +  GetFullyQualifiedName("LocalResourceLang") + " rl ON r.LocalResourceId = rl.LocalResourceId"+
                " WHERE r.PortalId = @PortalId" +
                " AND r.LocalResourceToken = @LocalResourceToken" +
                " AND rl.Language = @Language";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("PortalId",portalId), 
                                               new SqlParameter("Language", language), 
                                               new SqlParameter("LocalResourceToken", token), 
                                           };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override IDataReader GetLocalResourceLangs(int resourceId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("LocalResourceLang") + 
                " WHERE LocalResourceId = @LocalResourceId";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("LocalResourceId",resourceId), 
                                           };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override IDataReader GetLocalResourceLangs(int portalId, string token)
        {
            string sqlCmd = "SELECT rl.* FROM " + GetFullyQualifiedName("LocalResource") + " r " +
                            " INNER JOIN " + GetFullyQualifiedName("LocalResourceLang") + " rl ON r.LocalResourceId = rl.LocalResourceId" +
                            " WHERE r.PortalId = @PortalId" +
                            " AND r.Token = @Token";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("PortalId",portalId), 
                                               new SqlParameter("Token", token), 
                                           };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
            
        }
        public override void NewLocalResourceLang(LocalResourceLangInfo resourceLangInfo)
        {
            string sqlCmd = "INSERT INTO " + GetFullyQualifiedName("LocalResourceLang") + 
                " (LocalResourceId,Language,TextValue) VALUES (@LocalResourceId,@Language,@TextValue)";
            
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("LocalResourceId",resourceLangInfo.LocalResourceId), 
                                               new SqlParameter("Language", resourceLangInfo.Language), 
                                               new SqlParameter("TextValue", resourceLangInfo.TextValue), 
                                           };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override void UpdateLocalResourceLang(LocalResourceLangInfo resourceLangInfo)
        {
            string sqlCmd = "UPDATE " + GetFullyQualifiedName("LocalResourceLang") + " SET TextValue = @TextValue " +
                            " WHERE Language = @Language AND LocalResourceId = @LocalResourceId";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("LocalResourceId",resourceLangInfo.LocalResourceId), 
                                               new SqlParameter("Language", resourceLangInfo.Language), 
                                               new SqlParameter("TextValue", resourceLangInfo.TextValue), 
                                           };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
            
        }
        public override void DeleteLocalResourceLang(LocalResourceLangInfo resourceLangInfo)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("LocalResourceLang") + 
                            " WHERE Language = @Language AND LocalResourceId = @LocalResourceId";

            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("LocalResourceId",resourceLangInfo.LocalResourceId), 
                                               new SqlParameter("Language", resourceLangInfo.Language), 
                                               new SqlParameter("TextValue", resourceLangInfo.TextValue), 
                                           };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
            
        }
        public override void DeleteLocalResourceLangs(int resourceId)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("LocalResourceLang") +
                            " WHERE LocalResourceId = @LocalResourceId";

            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("LocalResourceId",resourceId), 
                                           };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        // ContactAddress methods
        public override IDataReader GetContactAddresses(DateTime? StartDate)
        {
            if (StartDate != null)
            {
                string selCmd = "SELECT *" +
                    " FROM " + GetFullyQualifiedName("ContactAddress") +
                    " WHERE CreatedOnDate >= @StartDate";
                return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("StartDate", StartDate));
            }
            else
            {
                string selCmd = "SELECT *" +
                    " FROM " + GetFullyQualifiedName("ContactAddress");
                return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
            }

        }
        public override int NewContactAddress(ContactAddressInfo ContactAddress)
        {
            string insCmd = "SET NOCOUNT ON INSERT INTO " + GetFullyQualifiedName("ContactAddress") +
                " (PortalID,Company,Prefix,Firstname,Lastname,Unit,Street,Region,Postalcode,City,Country,Telephone,Fax,Cell,Email)" +
                " VALUES " +
                " (@PortalID,@Company,@Prefix,@Firstname,@Lastname,@Unit,@Street,@Region,@Postalcode,@City,@Country,@Telephone,@Fax,@Cell,@Email)" +
                "  SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("PortalID",ContactAddress.PortalId),
                new SqlParameter("Company",ContactAddress.Company),
                new SqlParameter("Prefix",ContactAddress.Prefix),
                new SqlParameter("Firstname",ContactAddress.Firstname),
                new SqlParameter("Lastname",ContactAddress.Lastname),
                new SqlParameter("Unit",ContactAddress.Unit),
                new SqlParameter("Street",ContactAddress.Street),
                new SqlParameter("Region",ContactAddress.Region),
                new SqlParameter("Postalcode",ContactAddress.PostalCode),
                new SqlParameter("City",ContactAddress.City),
                new SqlParameter("Country",ContactAddress.Country),
                new SqlParameter("Telephone",ContactAddress.Telephone),
                new SqlParameter("Fax",ContactAddress.Fax),
                new SqlParameter("Cell",ContactAddress.Cell),
                new SqlParameter("Email",ContactAddress.Email)};

            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateContactAddress(ContactAddressInfo ContactAddress)
        {
            string updCmd = "UPDATE " + GetFullyQualifiedName("ContactAddress") + " SET " +
                " PortalID = @PortalID," +
                " Company = @Company," +
                " Prefix = @Prefix," +
                " Firstname = @Firstname," +
                " Lastname = @Lastname," +
                " Unit = @Unit," +
                " Street = @Street," +
                " Region = @Region," +
                " Postalcode = @Postalcode," +
                " City = @City," +
                " Country = @Country," +
                " Telephone = @Telephone," +
                " Fax = @Fax," +
                " Cell = @Cell," +
                " Email = @Email" +
                " WHERE ContactAddressId = @ContactAddressId";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("CartId",ContactAddress.ContactAddressId),
                new SqlParameter("PortalID",ContactAddress.PortalId),
                new SqlParameter("Company",ContactAddress.Company),
                new SqlParameter("Prefix",ContactAddress.Prefix),
                new SqlParameter("Firstname",ContactAddress.Firstname),
                new SqlParameter("Lastname",ContactAddress.Lastname),
                new SqlParameter("Unit",ContactAddress.Unit),
                new SqlParameter("Street",ContactAddress.Street),
                new SqlParameter("Region",ContactAddress.Region),
                new SqlParameter("Postalcode",ContactAddress.PostalCode),
                new SqlParameter("City",ContactAddress.City),
                new SqlParameter("Country",ContactAddress.Country),
                new SqlParameter("Telephone",ContactAddress.Telephone),
                new SqlParameter("Fax",ContactAddress.Fax),
                new SqlParameter("Email",ContactAddress.Email)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteContactAddress(int ContactAddressId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("ContactAddress") +
                " WHERE ContactAddressId = @ContactAddressId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("ContactAddressId", ContactAddressId));
        }

        // ContactProduct methods
        public override IDataReader GetContactProductsByCartId(int PortalId, Guid CartId, string Language)
        {
            string selCmd = "SELECT DISTINCT SimpleProduct.SimpleProductId, SimpleProduct.SubscriberId,SimpleProduct.SupplierId, " +
                " SimpleProduct.PortalId,SimpleProduct.Image, SimpleProduct.UnitCost, SimpleProduct.OriginalUnitCost," +
                " SimpleProduct.HideCost,SimpleProduct.TaxPercent,SimpleProduct.UnitId," +
                " SimpleProduct.ItemNo,SimpleProduct.CreatedOnDate,SimpleProduct.CreatedByUserId," +
                " SimpleProduct.LastModifiedOnDate,SimpleProduct.LastModifiedByUserId,SimpleProduct.Disabled," +
                " Lang.ShortDescription + ContactProduct.SelectedAttributes AS ShortDescription,Lang.ProductDescription, Lang.Attributes, Lang.Name" +
                " FROM " + GetFullyQualifiedName("SimpleProduct") + " SimpleProduct" +
                " INNER JOIN " + GetFullyQualifiedName("SimpleProductLang") + " Lang ON SimpleProduct.SimpleProductId = Lang.SimpleProductId" +
                " INNER JOIN " + GetFullyQualifiedName("ContactProduct") + " ContactProduct ON Simpleproduct.SimpleProductId = ContactProduct.ProductId" +
                " WHERE SimpleProduct.PortalId = @PortalId" +
                " AND Lang.Language = @Language " +
                " AND ContactProduct.CartId = @CartId";
            
            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("CartId",CartId),
                new SqlParameter("PortalId",PortalId),
                new SqlParameter("Language",Language)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetContactProductsByAddressId(int PortalId, int ContactAddressId, string Language)
        {
            string selCmd = "SELECT DISTINCT SimpleProduct.SimpleProductId, SimpleProduct.SubscriberId,SimpleProduct.SupplierId, " +
                " SimpleProduct.PortalId,SimpleProduct.Image, SimpleProduct.UnitCost, SimpleProduct.OriginalUnitCost," +
                " SimpleProduct.HideCost,SimpleProduct.TaxPercent,SimpleProduct.UnitId," +
                " SimpleProduct.ItemNo,SimpleProduct.CreatedOnDate,SimpleProduct.CreatedByUserId," +
                " SimpleProduct.LastModifiedOnDate,SimpleProduct.LastModifiedByUserId,SimpleProduct.Disabled," +
                " Lang.ShortDescription,Lang.ProductDescription, Lang.Attributes, Lang.Name" +
                " FROM " + GetFullyQualifiedName("SimpleProduct") + " SimpleProduct" +
                " INNER JOIN " + GetFullyQualifiedName("SimpleProductLang") + " Lang ON SimpleProduct.SimpleProductId = Lang.SimpleProductId" +
                " INNER JOIN " + GetFullyQualifiedName("ContactProduct") + " ContactProduct ON Simpleproduct.SimpleProductId = ContactProduct.ProductId" +
                " WHERE SimpleProduct.PortalId = @PortalId" +
                " AND Lang.Language = @Language " +
                " AND ContactProduct.ContactAddressId = @ContactAddressId";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("ContactAddressId",ContactAddressId),
                new SqlParameter("PortalId",PortalId),
                new SqlParameter("Language",Language)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override void NewContactProduct(Guid CartId, int ProductId, int ContactAddressId, string selectedAttributes)
        {
            string insCmd = "INSERT INTO " + GetFullyQualifiedName("ContactProduct") +
                " (CartId,ProductId, ContactAddressId, SelectedAttributes)" +
                " VALUES " +
                " (@CartId,@ProductId,@ContactAddressId, @SelectedAttributes)";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("CartId",CartId),
                new SqlParameter("ContactAddressId",ContactAddressId),
                new SqlParameter("ProductId",ProductId),
                new SqlParameter("SelectedAttributes", selectedAttributes), 
            };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateContactProduct(Guid CartId, int ProductId, int ContactAddressId)
        {
            string updCmd = "UPDATE " + GetFullyQualifiedName("ContactProduct") + " SET " +
                            " CartId = NULL," +
                            " ContactAddressId = @ContactAddressId" +
                            " WHERE CartId = @CartId" +
                            " AND ProductId = @ProductId";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("CartId",CartId),
                new SqlParameter("ContactAddressId",ContactAddressId),
                new SqlParameter("ProductId",ProductId),
            };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteContactProduct(Guid CartId, int ProductId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("ContactProduct") +
                " WHERE CartId = @CartId";
            if (ProductId > -1)
                delCmd += " AND ProductId = @ProductId";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("CartId",CartId),
                new SqlParameter("ProductId",ProductId)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
        }
        public override void DeleteContactProduct(int ContactAddressId, int ProductId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("ContactProduct") +
                " WHERE ContactAddressId = @ContactAddressId";
            if (ProductId > -1)
                delCmd += " AND ProductId = @ProductId";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("ContactAddressId",ContactAddressId),
                new SqlParameter("ProductId",ProductId)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
        }

        // ContactReason methods
        public override IDataReader GetContactReasons(int ContactAddressId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("ContactReason") +
                " WHERE ContactAddressId = @ContactAddressId";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("ContactAddressId", ContactAddressId));
        }
        public override IDataReader GetContactReasonByToken(int ContactAddressId, string Token)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("ContactReason") +
                " WHERE ContactAddressId = @ContactAddressId" +
                " AND Token = @Token";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("ContactAddressId",ContactAddressId),
                new SqlParameter("Token", Token)};

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override void NewContactReason(ContactReasonInfo ContactReason)
        {
            string insCmd = "INSERT INTO " + GetFullyQualifiedName("ContactReason") +
                " (ContactAddressId,Reason,Token)" +
                " VALUES " +
                " (@ContactAddressId,@Reason,@Token)";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("ContactAddressId",ContactReason.ContactAddressId),
                new SqlParameter("Reason",ContactReason.Reason),
                new SqlParameter("Token",ContactReason.Token)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateContactReason(ContactReasonInfo ContactReason)
        {
             string updCmd = "UPDATE " + GetFullyQualifiedName("ContactReason") +" SET " + 
                 " Reason = @Reason" +
                 " WHERE ContactAddressId = @ContactAddressId " +
                 " AND Token = @Token" ;

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("ContactAddressId",ContactReason.ContactAddressId),
                new SqlParameter("Reason",ContactReason.Reason),
                new SqlParameter("Token",ContactReason.Token)};

             SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteContactReasons(int ContactAddressId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("ContactReason") +
                " WHERE ContactAddressId = @ContactAddressId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("ContactAddressId", ContactAddressId));
        }
        public override void DeleteContactReason(int ContactAddressId, string Token)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("ContactReason") +
                " WHERE ContactAddressId = @ContactAddressId" +
                " AND Token = @Token";
            
            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("ContactAddressId",ContactAddressId),
                new SqlParameter("Token",Token)};

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
        }

        // Unit methods
        public override IDataReader GetUnits(int PortalId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("Unit") +
                " WHERE PortalId = @PortalId" +
                " ORDER BY UnitId ASC";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("PortalId", PortalId));
        }

        public override IDataReader GetUnits(int PortalId, string Language, string sortByField)
        {
            string selCmd = "SELECT Unit.*," +
                " Lang.Unit,Lang.Symbol" +
                " FROM " + GetFullyQualifiedName("Unit") + " Unit" +
                " INNER JOIN " + GetFullyQualifiedName("UnitLang") + " Lang ON Unit.UnitId = Lang.UnitId" +
                " WHERE Unit.PortalId = @PortalId" +
                " AND Lang.Language = @Language" +
                " ORDER BY " + sortByField + " ASC";

            SqlParameter[] SqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("PortalId", PortalId),
                                               new SqlParameter("Language", Language)
                                           };

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override IDataReader GetUnit(int UnitId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("Unit") +
                " WHERE UnitId = @UnitId" +
                " ORDER BY UnitId DESC";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("UnitId", UnitId));
        }
        public override IDataReader GetUnit(int UnitId, string Language)
        {
            string selCmd = "SELECT Unit.*," +
                " Lang.Unit,Lang.Symbol" +
                " FROM " + GetFullyQualifiedName("Unit") + " Unit" +
                " INNER JOIN " + GetFullyQualifiedName("UnitLang") + " Lang ON Unit.UnitId = Lang.UnitId" +
                " WHERE Unit.UnitId = @UnitId" +
                " AND Lang.Language = @Language" +
                " ORDER BY Unit.UnitId DESC";

            SqlParameter[] SqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("UnitId", UnitId),
                                               new SqlParameter("Language", Language)
                                           };

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
        }
        public override int NewUnit(UnitInfo Unit)
        {
            string insCmd = "SET NOCOUNT ON INSERT INTO " + GetFullyQualifiedName("Unit") +
                " (PortalId,Decimals)" +
                " VALUES " +
                " (@PortalId,@Decimals) SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter[] SqlParams = new SqlParameter[] {
        new SqlParameter("UnitId",Unit.UnitId),
        new SqlParameter("PortalId",Unit.PortalId),
        new SqlParameter("Decimals",Unit.Decimals)};

            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateUnit(UnitInfo Unit)
        {
            string updCmd = "UPDATE " + GetFullyQualifiedName("Unit") + " SET " +
                " PortalId = @PortalId," +
                " Decimals = @Decimals" +
                " WHERE UnitId = @UnitId";

            SqlParameter[] SqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("UnitId", Unit.UnitId),
                                               new SqlParameter("PortalId", Unit.PortalId),
                                               new SqlParameter("Decimals", Unit.Decimals)
                                           };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteUnit(int UnitId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("Unit") +
                " WHERE UnitId = @UnitId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("UnitId", UnitId));
        }

        // UnitLang methods
        public override IDataReader GetUnitLang(int unitId, string language)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("UnitLang") +
                            " WHERE UnitId = @UnitId AND Language = @Language";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("UnitId", unitId), 
                                               new SqlParameter("Language", language), 
                                           };
            return (IDataReader) SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override IDataReader GetUnitLangs(int unitId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("UnitLang") +
                            " WHERE UnitId = @UnitId ";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("UnitId", unitId), 
                                           };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        public override IDataReader GetUnitLangsByPortal(int portalId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("UnitLang") +
                            " WHERE UnitId IN (SELECT UnitId FROM " + GetFullyQualifiedName("Unit") + " WHERE PortalId = @PortalId)";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("PortalId", portalId), 
                                           };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override IDataReader GetPortalUnitLangs(int portalId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("UnitLang") +
                            " WHERE PortalId = @PortalId";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("PortalId", portalId), 
                                           };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override void NewUnitLang(UnitLangInfo unitLang)
        {
            string sqlCmd = "INSERT INTO " + GetFullyQualifiedName("UnitLang") +
                            " (UnitId, Language, Unit, Symbol) VALUES (@UnitId, @Language, @Unit, @Symbol)";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("UnitId", unitLang.UnitId), 
                                               new SqlParameter("Language", unitLang.Language), 
                                               new SqlParameter("Unit", unitLang.Unit), 
                                               new SqlParameter("Symbol", unitLang.Symbol), 
                                           };
           SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        public override void DeleteUnitLang(int unitId, string language)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("UnitLang") +
                            " WHERE UnitId= @UnitId AND Language=@Language";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("UnitId", unitId), 
                                               new SqlParameter("Language", language), 
                                           };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override void DeleteUnitLangs(int unitId)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("UnitLang") +
                            " WHERE UnitId= @UnitId";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("UnitId", unitId), 
                                           };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        // ShippingModel
        public override IDataReader GetShippingModel(int shippingModelId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("ShippingModel") +
                            " WHERE ShippingModelId = @ShippingModelId";
            SqlParameter[] sqlParams = new SqlParameter[]
                                       {
                                           new SqlParameter("ShippingModelId", shippingModelId), 
                                       };
            return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        public override IDataReader GetShippingModels(int portalId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("ShippingModel") +
                            " WHERE PortalId = @PortalId";
            SqlParameter[] sqlParams = new SqlParameter[]
                                       {
                                           new SqlParameter("PortalId", portalId), 
                                       };
            return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        public override int NewShippingModel(ShippingModelInfo shippingModel)
        {
            string sqlCmd = "SET NOCOUNT ON INSERT INTO " + GetFullyQualifiedName("ShippingModel") +
                            " (PortalId,Tax,Enabled,Name) VALUES (@PortalId,@Tax,@Enabled,@Name) SELECT CAST(scope_identity() AS INTEGER);";
            SqlParameter[] sqlParams = new SqlParameter[]
            {
                new SqlParameter("PortalId", shippingModel.PortalId),
                new SqlParameter("Tax", shippingModel.Tax),
                new SqlParameter("Enabled", shippingModel.Enabled),
                new SqlParameter("Name", shippingModel.Name),
            };

            return (int) SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd, sqlParams);

        }

        public override void UpdateShippingModel(ShippingModelInfo shippingModel)
        {
            string sqlCmd = " UPDATE " + GetFullyQualifiedName("ShippingModel") + " SET" +
                            " PortalId = @PortalId," +
                            " Tax = @Tax," +
                            " Enabled = @Enabled," +
                            " Name = @Name" +
                            " WHERE ShippingModelId = @ShippingModelId";
            SqlParameter[] sqlParams = new SqlParameter[]
            {
                new SqlParameter("ShippingModelId", shippingModel.ShippingModelID),
                new SqlParameter("PortalId", shippingModel.PortalId),
                new SqlParameter("Tax", shippingModel.Tax),
                new SqlParameter("Enabled", shippingModel.Enabled),
                new SqlParameter("Name", shippingModel.Name),
            };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);

        }

        public override void DeleteShippingModel(int shippingModelId)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("ShippingModel") +
                            " WHERE ShippingModelId = @ShippingModelId";
            SqlParameter[] sqlParams = new SqlParameter[]
                                       {
                                           new SqlParameter("ShippingModelId", shippingModelId), 
                                       };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        
        // ProductShippingModel
        public override IDataReader GetProductShippingModelsByProduct(int productId)
        {
            string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("ProductShippingModel") +
                            " WHERE SimpleProductId = @ProductId";

            SqlParameter[] sqlParams = new SqlParameter[]
                                       {
                                           new SqlParameter("ProductId", productId), 
                                       };

            return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        public override void DeleteProductShippingModelByProduct(int productId)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("ProductShippingModel") +
                            " WHERE SimpleProductId = @ProductId";

            SqlParameter[] sqlParams = new SqlParameter[]
                                       {
                                           new SqlParameter("ProductId", productId), 
                                       };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        public override void InsertProductShippingModel(ProductShippingModelInfo productShippingModel)
        {
            string sqlCmd = "INSERT INTO " + GetFullyQualifiedName("ProductShippingModel") +
                            " (ShippingModelId,SimpleProductId) VALUES (@ShippingModelId,@ProductId)";

            SqlParameter[] sqlParams = new SqlParameter[]
                                       {
                                           new SqlParameter("ProductId", productShippingModel.SimpleProductId), 
                                           new SqlParameter("ShippingModelId", productShippingModel.ShippingModelId), 
                                       };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        // ShippingCost methods
        public override IDataReader GetShippingCosts(int PortalId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("ShippingCost") +
                " WHERE PortalId = @PortalId" +
                " ORDER BY ShippingCostId DESC";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("PortalId", PortalId));
        }
        public override IDataReader GetShippingCostsByModelId(int shippingModelId)
        {
            string selCmd = "SELECT *" +
                            " FROM " + GetFullyQualifiedName("ShippingCost") +
                            " WHERE shippingModelId = @ShippingModelId";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("ShippingModelId", shippingModelId));
        }
        public override IDataReader GetShippingCostById(int ShippingCostId)
        {
            string selCmd = "SELECT *" +
                " FROM " + GetFullyQualifiedName("ShippingCost") +
                " WHERE ShippingCostId = @ShippingCostId" +
                " ORDER BY ShippingCostId DESC";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("ShippingCostId", ShippingCostId));
        }
        public override int NewShippingCost(ShippingCostInfo shippingCost)
        {
            string insCmd = "SET NOCOUNT ON INSERT INTO " + GetFullyQualifiedName("ShippingCost") +
                " (ShippingModelID,ShippingZoneID,ShippingPrice,PerPart,MinWeight,MaxWeight,MinPrice,MaxPrice)" +
                " VALUES " +
                " (@ShippingModelID,@ShippingZoneID,@ShippingPrice,@PerPart,@MinWeight,@MaxWeight,@MinPrice,@MaxPrice) SELECT CAST(scope_identity() AS INTEGER);";

            SqlParameter[] SqlParams = new SqlParameter[]
                                       {
                                           new SqlParameter("ShippingCostID", shippingCost.ShippingCostID),
                                           new SqlParameter("ShippingModelID", shippingCost.ShippingModelID),
                                           new SqlParameter("ShippingZoneID", shippingCost.ShippingZoneID),
                                           new SqlParameter("ShippingPrice", shippingCost.ShippingPrice),
                                           new SqlParameter("PerPart", shippingCost.PerPart),
                                           new SqlParameter("MinWeight", shippingCost.MinWeight),
                                           new SqlParameter("MaxWeight", shippingCost.MaxWeight),
                                           new SqlParameter("MinPrice", shippingCost.MinPrice),
                                           new SqlParameter("MaxPrice", shippingCost.MaxPrice)
                                       };

            return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, insCmd, SqlParams);
        }
        public override void UpdateShippingCost(ShippingCostInfo shippingCost)
        {
            string updCmd = "UPDATE " + GetFullyQualifiedName("ShippingCost") + " SET " +
                " ShippingModelID = @ShippingModelID," +
                " ShippingZoneID = @ShippingZoneID," +
                " ShippingPrice = @ShippingPrice," +
                " PerPart = @PerPart," +
                " MinWeight = @MinWeight," +
                " MaxWeight = @MaxWeight," +
                " MinPrice = @MinPrice," +
                " MaxPrice = @MaxPrice" +
                " WHERE ShippingCostID = @ShippingCostID";

            SqlParameter[] SqlParams = new SqlParameter[]
                                       {
                                           new SqlParameter("ShippingCostID", shippingCost.ShippingCostID),
                                           new SqlParameter("ShippingModelID", shippingCost.ShippingModelID),
                                           new SqlParameter("ShippingZoneID", shippingCost.ShippingZoneID),
                                           new SqlParameter("ShippingPrice", shippingCost.ShippingPrice),
                                           new SqlParameter("PerPart", shippingCost.PerPart),
                                           new SqlParameter("MinWeight", shippingCost.MinWeight),
                                           new SqlParameter("MaxWeight", shippingCost.MaxWeight),
                                           new SqlParameter("MinPrice", shippingCost.MinPrice),
                                           new SqlParameter("MaxPrice", shippingCost.MaxPrice)
                                       };

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
        }
        public override void DeleteShippingCost(int ShippingCostId)
        {
            string delCmd = "DELETE FROM " + GetFullyQualifiedName("ShippingCost") +
                " WHERE ShippingCostId = @ShippingCostId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("ShippingCostId", ShippingCostId));
        }

        // ShippingZone methods
        public override int GetShippingZoneIdByAddress(int modelId, string countryCodeISO2, int postalCode)
        {
            int shippingZoneId = -1;
            string sqlCmd = "SELECT ShippingZoneId" +
                            " FROM " + GetFullyQualifiedName("ShippingZone") + 
                            " WHERE ShippingModelId = @ShippingModelId" +
                            " AND ShippingZoneId IN " +
                            "   (SELECT ShippingZoneId FROM " + GetFullyQualifiedName("ShippingArea") + " WHERE {0})";

            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("CountryCodeISO2",countryCodeISO2), 
                                               new SqlParameter("PostalCode", postalCode), 
                                               new SqlParameter("ShippingModelId", modelId), 
                                           };

            string where = "";
            object result = null;
            if (postalCode > -1)
            {
                where = "CountryCodeISO2 = @CountryCodeISO2 AND PostalCodeMin < @PostalCode AND PostalCodeMax > @PostalCode";
                result = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, String.Format(sqlCmd, where), sqlParams);
                if (result != null && result != DBNull.Value)
                {
                    return (int) result;
                }
            }
            where = "CountryCodeISO2 = @CountryCodeISO2";
            result = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, String.Format(sqlCmd, where), sqlParams);
            if (result != null && result != DBNull.Value)
            {
                return (int)result;
            }
            return -1;
        }

        public override IDataReader GetShippingZoneById(int shippingZoneId, string language)
        {
            string sqlCmd = "SELECT sz.*, lang.Name, lang.OrderText,lang.Description" +
                            " FROM " + GetFullyQualifiedName("ShippingZone") + " sz " +
                            " INNER JOIN " + GetFullyQualifiedName("ShippingZoneLang") + " lang ON sz.ShippingZoneId = lang.ShippingZoneId" +
                            " WHERE sz.ShippingZoneId = @ShippingZoneId " +
                            " AND lang.Language = @Language";
            SqlParameter[] sqlParams = new SqlParameter[]
                                           {
                                               new SqlParameter("ShippingZoneId",shippingZoneId), 
                                               new SqlParameter("Language", language), 
                                           };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        // Helper methods
        public override void ReseedTables()
        {
            string sqlCmd = "SELECT MAX(ProductGroupId) FROM " + GetFullyQualifiedName("ProductGroup");
            object result = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd);
            if (result != DBNull.Value)
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DBCC CHECKIDENT ('" + GetFullyQualifiedName("ProductGroup") +"', RESEED," + ((int)result + 1).ToString() + ")");
            else
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DBCC CHECKIDENT ('" + GetFullyQualifiedName("ProductGroup") + "', RESEED, 1)");

            sqlCmd = "SELECT MAX(SimpleProductId) FROM " + GetFullyQualifiedName("SimpleProduct");
            result = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd);
            if (result != DBNull.Value)
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DBCC CHECKIDENT ('" + GetFullyQualifiedName("SimpleProduct") + "', RESEED," + ((int)result + 1).ToString() + ")");
            else
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DBCC CHECKIDENT ('" + GetFullyQualifiedName("SimpleProduct") + "', RESEED, 1)");

            sqlCmd = "SELECT MAX(FeatureGroupId) FROM " + GetFullyQualifiedName("FeatureGroup");
            result = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd);
            if (result != DBNull.Value)
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DBCC CHECKIDENT ('" + GetFullyQualifiedName("FeatureGroup") + "', RESEED," + ((int)result + 1).ToString() + ")");
            else
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DBCC CHECKIDENT ('" + GetFullyQualifiedName("FeatureGroup") + "', RESEED, 1)");

            sqlCmd = "SELECT MAX(FeatureId) FROM " + GetFullyQualifiedName("Feature");
            result = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd);
            if (result != DBNull.Value)
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DBCC CHECKIDENT ('" + GetFullyQualifiedName("Feature") + "', RESEED," + ((int)result + 1).ToString() + ")");
            else
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DBCC CHECKIDENT ('" + GetFullyQualifiedName("Feature") + "', RESEED, 1)");

            sqlCmd = "SELECT MAX(FeatureValueId) FROM " + GetFullyQualifiedName("FeatureValue");
            result = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd);
            if (result != DBNull.Value)
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DBCC CHECKIDENT ('" + GetFullyQualifiedName("FeatureValue") + "', RESEED," + ((int)result + 1).ToString() + ")");
            else
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DBCC CHECKIDENT ('" + GetFullyQualifiedName("FeatureValue") + "', RESEED, 1)");

            sqlCmd = "SELECT MAX(FeatureListId) FROM " + GetFullyQualifiedName("FeatureList");
            result = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd);
            if (result != DBNull.Value)
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DBCC CHECKIDENT ('" + GetFullyQualifiedName("FeatureList") + "', RESEED," + ((int)result + 1).ToString() + ")");
            else
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DBCC CHECKIDENT ('" + GetFullyQualifiedName("FeatureList") + "', RESEED, 1)");

            sqlCmd = "SELECT MAX(FeatureListItemId) FROM " + GetFullyQualifiedName("FeatureListItem");
            result = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd);
            if (result != DBNull.Value)
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DBCC CHECKIDENT ('" + GetFullyQualifiedName("FeatureListItem") + "', RESEED," + ((int)result + 1).ToString() + ")");
            else
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DBCC CHECKIDENT ('" + GetFullyQualifiedName("FeatureListItem") + "', RESEED, 1)");
        }

        #endregion

        #region SearchFilters
        public override string GetSearchTextFilter(int PortalId, string SearchText, string Language)
        {
            return "(SimpleProduct.SimpleProductId IN " +
                   " (SELECT SimpleProductId FROM " + GetFullyQualifiedName("SimpleProductLang") + " SimpleProductLang " +
                   " WHERE (Name Like '%" + SearchText + "%' OR ShortDescription LIKE '%" + SearchText + "%' OR ProductDescription LIKE '%" + SearchText + "%') " +
                   " AND Language = '" + Language + "')" +
                   " OR Simpleproduct.ItemNo Like '%" + SearchText + "%')";
        }
        public override string GetSearchStaticFilter(int PortalId, string Token, string Language)
        {
            string selCmd = "SELECT FilterCondition" +
                " FROM " + GetFullyQualifiedName("StaticFilter") +
                " WHERE Token = @Token" +
                " And PortalId = @PortalId";

            SqlParameter[] SqlParams = new SqlParameter[] {
                new SqlParameter("PortalId", PortalId),
                new	SqlParameter("Token",Token)};

            string retVal =  (string)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd, SqlParams);
            retVal = retVal.Replace("[Language]", Language);
            retVal = retVal.Replace("[Date]", DateTime.Now.ToString());
            return retVal;
        }
        public override string GetSearchStaticFilter(int StaticFilterId, string Language)
        {
            string selCmd = "SELECT FilterCondition" +
                " FROM " + GetFullyQualifiedName("StaticFilter") +
                " WHERE StaticFilterId = @StaticfilterId";

            string retVal = (string)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd,new SqlParameter("StaticFilterId",StaticFilterId));
            retVal = retVal.Replace("[Language]", Language);
            retVal = retVal.Replace("[Date]", DateTime.Now.ToString());
            return retVal;
        }
        public override string GetSearchPriceFilter(int PortalId, decimal StartPrice, decimal EndPrice, bool IncludeTax)
        {
            if (IncludeTax)
                return "SimpleProduct.SimpleProductId IN " +
                       " (SELECT SimpleProductId FROM " + GetFullyQualifiedName("SimpleProduct") + " SimpleProduct " +
                       " WHERE UnitCost * (100 + TaxPercent) / 100 BETWEEN " + 
                       StartPrice.ToString(CultureInfo.InvariantCulture) + 
                       " AND " + EndPrice.ToString(CultureInfo.InvariantCulture) + ")";
            else
                return "SimpleProduct.SimpleProductId IN " +
                       " (SELECT SimpleProductId FROM " + GetFullyQualifiedName("SimpleProduct") + " SimpleProduct " +
                       " WHERE UnitCost BETWEEN " + StartPrice.ToString(CultureInfo.InvariantCulture) + 
                       " AND " + EndPrice.ToString(CultureInfo.InvariantCulture) + ")";

        }
        public override string GetProductGroupFilter(int PortalId, int ProductGroupId, bool IncludeChilds)
        {
            if (ProductGroupId == -1)
            {
                if (IncludeChilds)
                    return "";
                else
                    return " SimpleProduct.SimpleProductId NOT IN (SELECT SimpleProductId FROM " + Prefix + "ProductInGroup)";
            }
            else
            {
                if (IncludeChilds)
                {
                    string selCmd =
                        "with cte as (" +
                        "    select ProductGroupId FROM " + Prefix + "ProductGroup " +
                        "      WHERE PortalId = " + PortalId.ToString() +
                        "      AND ParentId = " + ProductGroupId.ToString() +
                        "    union   " +
                        "    Select ProductGroupId FROM " + Prefix + "ProductGroup WHERE ProductGroupId = " + ProductGroupId.ToString() +
                        "    union all " +
                        "    select pg.ProductGroupId " +
                        "    from " + Prefix + "ProductGroup pg" +
                        "    inner join cte on cte.ProductGroupId = pg.ParentId" +
                        " ) " +
                        "select Distinct ProductGroupId from cte";
                    IDataReader dr = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
                    string idList = "";
                    while (dr.Read())
                    {
                        idList += dr["ProductGroupId"].ToString() + ",";
                    }
                    dr.Close();
                    if (!String.IsNullOrEmpty(idList))
                        return " SimpleProduct.SimpleProductId IN ( SELECT SimpleProductId FROM " + Prefix +
                            "ProductInGroup WHERE ProductGroupId IN ( " + idList.Substring(0, idList.Length - 1) + "))";
                    else
                        return " SimpleProduct.SimpleProductId IN (SELECT SimpleProductId FROM " + Prefix +
                            "ProductInGroup WHERE ProductGroupId = " + ProductGroupId.ToString() + ")";
                }
                else
                    return " SimpleProduct.SimpleProductId IN (SELECT SimpleProductId FROM " + Prefix +
                        "ProductInGroup WHERE ProductGroupId = " + ProductGroupId.ToString() + ")";
            }
        }
        public override string GetSearchFeatureFilter(string DataType, string Value)
        {
            string retVal = "";
            if (DataType == "L")
            {
                string[] allValues = Value.Split('|');
                foreach (string value in allValues)
                {
                    if (retVal != string.Empty)
                        retVal += " AND ";
                    retVal += "SimpleProduct.SimpleProductId IN " +
                              " ( SELECT ProductId FROM " + GetFullyQualifiedName("FeatureValue") +
                              "   WHERE FeatureListItemId =" + value + ")";
                }
            }
            else
                retVal = "SimpleProduct.SimpleProductId IN " +
                    " ( SELECT ProductId FROM " + GetFullyQualifiedName("FeatureValue");
                switch (DataType)
                {
                    case "I":
                        retVal += " WHERE iValue = " + Value +")";
                        break;
                    case "N":
                        retVal += " WHERE nValue = " + Value+")";
                        break;
                    case "C":
                        retVal += " WHERE cValue LIKE '%" + Value + "%')";
                        break;
                    case "F":
                        retVal += " WHERE fValue = " + Value + ")";
                        break;
                    case "B":
                        retVal += " WHERE bValue = " + (Value.ToUpper() == "TRUE" ? "1" : "0") + ")";
                        break;
                    case "T":
                        retVal += " WHERE tValue = '" + Value + "')";
                        break;
                }
                return retVal;
        }

        public override string GetSearchFeatureListFilter(int FeatureListId, int FeatureListItemId)
        {
            string retVal = "SimpleProduct.SimpleProductId IN " +
                " ( SELECT ProductId FROM " + GetFullyQualifiedName("FeatureValue") + " fv" +
                "   INNER JOIN " + GetFullyQualifiedName("Feature") + " f ON fv.FeatureId = f.FeatureId" +
                "   WHERE f.FeatureListId = " + FeatureListId.ToString() +
                "   AND fv.FeatureListItemId = " + FeatureListItemId.ToString() + ")";
            return retVal;

        }
        #endregion

        #region ImportRelation
        
        public override IDataReader GetImportRelationOwnIdsByTable(int portalId, string tableName, Guid storeGuid)
        {
            string sqlCmd = "SELECT OwnId FROM " + GetFullyQualifiedName("ImportRelation") +
                " WHERE PortalId = @PortalId AND Tablename = @Tablename AND StoreGuid = @StoreGuid ORDER By OwnId Desc";

            SqlParameter[] sqlParams = new SqlParameter[]
                                       {
                                           new SqlParameter("PortalId", portalId),
                                           new SqlParameter("Tablename", tableName),
                                           new SqlParameter("StoreGuid", storeGuid)
                                       };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }
        public override IDataReader GetImportRelationForeignIdsByTable(int portalId, string tableName, Guid storeGuid)
        {
            string sqlCmd = "SELECT ForeignId FROM " + GetFullyQualifiedName("ImportRelation") +
                " WHERE PortalId = @PortalId AND Tablename = @Tablename AND StoreGuid = @StoreGuid ORDER By OwnId Desc";

            SqlParameter[] sqlParams = new SqlParameter[]
                                       {
                                           new SqlParameter("PortalId", portalId),
                                           new SqlParameter("Tablename", tableName),
                                           new SqlParameter("StoreGuid", storeGuid)
                                       };
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
        }

        public override int GetImportRelationOwnId(int PortalId, string Tablename, int ForeignId, Guid storeGuid)
        {
            string selCmd = "SELECT OwnId FROM " + GetFullyQualifiedName("ImportRelation") +
                " WHERE PortalId = @PortalId AND Tablename = @Tablename AND ForeignId = @ForeignId AND StoreGuid = @StoreGuid ";

            SqlParameter[] param = new SqlParameter[]
                                       {
                                           new SqlParameter("PortalId", PortalId),
                                           new SqlParameter("Tablename", Tablename),
                                           new SqlParameter("ForeignId", ForeignId),
                                           new SqlParameter("StoreGuid", storeGuid), 
                                       };
            object ret = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd, param);
            return (ret == null ? -1 : (int)ret);
        }
        
        public override int GetImportRelationForeignId(int PortalId, string Tablename, int OwnId, Guid storeGuid)
        {
            string selCmd = "SELECT ForeignId FROM " + GetFullyQualifiedName("ImportRelation") +
                " WHERE PortalId = @PortalId AND Tablename = @Tablename AND OwnId = @OwnId AND StoreGuid = @StoreGuid";

            SqlParameter[] param = new SqlParameter[]
                                       {
                                           new SqlParameter("PortalId", PortalId),
                                           new SqlParameter("Tablename", Tablename),
                                           new SqlParameter("OwnId", OwnId),
                                           new SqlParameter("StoreGuid",storeGuid), 
                                       };
            object ret = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd, param);
            return (ret == null ? -1 : (int)ret);
        }
        public override void NewImportRelation(int PortalId, string Tablename, int OwnId, int ForeignId, Guid storeGuid)
        {
            string insCmd = "INSERT INTO " + GetFullyQualifiedName("ImportRelation") +
                "(PortalId,TableName,OwnId,ForeignId, StoreGuid) VALUES (@PortalId,@TableName,@OwnId,@ForeignId, @StoreGuid)";
            SqlParameter[] param = new SqlParameter[]
                                       {
                                           new SqlParameter("PortalId", PortalId),
                                           new SqlParameter("Tablename", Tablename),
                                           new SqlParameter("OwnId", OwnId),
                                           new SqlParameter("ForeignId", ForeignId),
                                           new SqlParameter("StoreGuid", storeGuid), 
                                       };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd, param);
                
        }
        public override void DeleteImportRelationByOwnId(int PortalId, string Tablename, int OwnId)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("ImportRelation") +
                " WHERE PortalId = @PortalId AND Tablename = @Tablename AND OwnId = @OwnId";

            SqlParameter[] param = new SqlParameter[]{
                         new SqlParameter("PortalId",PortalId),
                         new SqlParameter("Tablename",Tablename),
                         new SqlParameter("OwnId",OwnId)};
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, param);

        }
        public override void DeleteImportRelationByForeignId(int PortalId, string Tablename, int ForeignId, Guid storeGuid)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("ImportRelation") +
                " WHERE PortalId = @PortalId AND Tablename = @Tablename AND ForeignId = @ForeignId AND StoreGuid = @StoreGuid";

            SqlParameter[] param = new SqlParameter[]
                                       {
                                           new SqlParameter("PortalId", PortalId),
                                           new SqlParameter("Tablename", Tablename),
                                           new SqlParameter("ForeignId", ForeignId),
                                           new SqlParameter("StoreGuid", storeGuid), 
                                       };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, param);
        }
        public override void DeleteImportRelationByTable(int PortalId, string Tablename, Guid storeGuid)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("ImportRelation") +
                " WHERE PortalId = @PortalId AND Tablename = @Tablename AND StoreGuid = @StoreGuid";

            SqlParameter[] param = new SqlParameter[]
                                       {
                                           new SqlParameter("PortalId", PortalId),
                                           new SqlParameter("Tablename", Tablename),
                                           new SqlParameter("StoreGuid", storeGuid), 

                                       };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, param);
        }
        public override void DeleteImportRelationByPortal(int PortalId)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("ImportRelation") +
                " WHERE PortalId = @PortalId";

            SqlParameter[] param = new SqlParameter[]{
                         new SqlParameter("PortalId",PortalId)};
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, param);
        }
        public override void DeleteImportRelationByStore(int PortalId, Guid storeGuid)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("ImportRelation") +
                " WHERE PortalId = @PortalId and StoreGuId = @StoreGuid";

            SqlParameter[] param = new SqlParameter[]
                                       {
                                           new SqlParameter("PortalId", PortalId),
                                           new SqlParameter("StoreGuid", storeGuid),
                                       };
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, param);
        }

        #endregion

        #region ImportStore

        public override void SaveImportStore(Guid storeGuid, string storeName)
        {
            string sqlCmd = "SELECT Count(*) FROM " + GetFullyQualifiedName("ImportStore") + " WHERE StoreGuid = @StoreGuid";
            int cnt = (int) SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("StoreGuid", storeGuid));
            SqlParameter[] sqParams = new SqlParameter[]
                                          {
                                              new SqlParameter("StoreGuid",storeGuid), 
                                              new SqlParameter("StoreName", storeName), 
                                          };
            if (cnt == 0)
                sqlCmd = "INSERT INTO " + GetFullyQualifiedName("ImportStore") + "(StoreGuid,StoreName) VALUES (@StoreGuid,@StoreName)";
            else
                sqlCmd = "UPDATE " + GetFullyQualifiedName("ImportStore") + " SET StoreName = @StoreName WHERE StoreGuid = @StoreGuid";

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqParams);
        }

        public override string GetImportStoreName(Guid storeGuid)
        {
            string sqlCmd = "SELECT StoreName FROM " + GetFullyQualifiedName("ImportStore") + " WHERE StoreGuid = @StoreGuid";
            return (string) SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("StoreGuid", storeGuid));
        }

        public override IDataReader GetStoreGuids(int PortalId)
        {
            string selCmd = "SELECT DISTINCT StoreGuid FROM " + GetFullyQualifiedName("ImportRelation") + " WHERE PortalId = @PortalId";
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("PortalId", PortalId));
        }

        public override void DeleteImportStore(Guid storeGuid)
        {
            string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("ImportStore") + " WHERE StoreGuid = @StoreGuid";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("StoreGuid", storeGuid));
        }
        #endregion
    }
}