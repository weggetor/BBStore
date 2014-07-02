BBStore_Cart: SELECT * FROM BBStore_Cart WHERE PortalId = @PORTALID
BBStore_CartAdditionalCost : SELECT * FROM BBStore_CartAdditionalCost WHERE CartID IN (SELECT CartID FROM BBSTore_Cart WHERE PortalId = @PORTALID)
BBStore_CartProduct: SELECT * FROM BBStore_CartProduct WHERE CartID IN (SELECT CartID FROM BBSTore_Cart WHERE PortalId = @PORTALID)
BBStore_CartProductOption: SELECT * FROM BBStore_CartProductOption WHERE CartProductID IN (SELECT CartProductId FROM BBSTore_CartProduct INNER JOIN BBStore_Cart ON BBStore_CartProduct.CartId = BBStore_Cart.CartID WHERE BBSTore_Cart.PortalId = @PORTALID)

BBStore_ContactAddress : SELECT * FROM BBStore_ContactAddress WHERE PortalId = @PORTALID
BBStore_ContactProduct: SELECT * FROM BBSTore_ContactProduct WHERE CartID IN (SELECT CartID FROM BBSTore_Cart WHERE PortalId = @PORTALID)
BBStore_ContactReason : SELECT * FROM BBStore_ContactReason WHERE ContactAddressId IN (SELECT ContactAddressId FROM BBStore_ContactAddress WHERE PortalId = @PORTALID)

BBStore_Customer: SELECT * FROM BBStore_Customer WHERE PortalId = @PortalId   (-> UserID !)
BBStore_CustomerAddress: SELECT * FROM BBStore_CustomerAddress WHERE PortalId = @PortalId
BBStore_CustomerPaymentProvider: SELECT * FROM BBStore_CustomerPaymentProvider WHERE CustomerID IN (SELECT CustomerID FROM BBStore_Customer WHERE PortalId = @PORTALID) 

BBStore_Feature: SELECT * FROM BBStore_Feature WHERE PortalId = @PORTALID
BBStore_FeatureGroup : SELECT * FROM BBStore_featureGroup WHERE PortalId = @PORTALID
BBSTore_FeatureGroupLang: SELECT * FROM BBStore_featureGroupLang WHERE FeatureGroupId IN (SELECT FeatureGroupId FROM BBStore_featureGroup WHERE PortalId = @PORTALID)
BBStore_FeatureLang: SELECT * FROM BBStore_FeatureLang WHERE FeatureID IN (SELECT FeatureID FROM BBStore_Feature WHERE PortalId = @PORTALID)
BBStore_FeatureList : SELECT * FROM BBStore_FeatureList WHERE PortalID = @PORTALID
BBStore_FeatureListItem : SELECT * FROM BBstore_FeatureListItem WHERE FeatureListID IN (SELECT FeatureListID FROM BBStore_FeatureList WHERE PortalID = @PORTALID)
BBStore_FeatureListItemLang : SELECT * FROM BBstore_FeatureListItemLang WHERE FeatureListItemID IN (SELECT FeatureListItemID FROM BBStore_FeatureListItem INNER JOIN BBStore_FeatureList ON BBStore_FeatureList.FeatureListID = BBStore_FeatureListItem.FeatureListID WHERE BBStore_FeatureList.PortalID = @PORTALID)
BBStore_FeatureListLang : SELECT * FROM BBStore_FeatureListLang WHERE FeatureListID IN (SELECT FeatureListID FROM BBStore_FeatureList WHERE PortalID = @PORTALID)
BBStore_FeatureValue: SELECT * FROM BBStore_FeatureValue WHERE FeatureID IN (SELECT FeatureID FROM BBStore_Feature WHERE PortalId = @PORTALID)

BBStore_ImportRelation: SELECT * FROM BBStore_ImportRelation WHERE PortalID = @PORTALID
BBStore_ModuleProduct: SELECT * FROM ModuleProduct WHERE PortalId = @PORTALID  (--> Module !!)
 
BBStore_Order: SELECT * FROM BBStore_Order WHERE PortalId = @PORTALID
BBStore_OrderAdditionalCost : SELECT * FROM BBStore_OrderAdditionalCost WHERE OrderID IN (SELECT OrderID FROM BBStore_Order WHERE PortalId = @PORTALID)
BBStore_OrderAddress : SELECT * FROM BBStore_OrderAddress WHERE PortalId = @PORTALID
BBStore_OrderProduct: SELECT * FROM BBStore_OrderProduct WHERE OrderID IN (SELECT OrderID FROM BBSTore_Order WHERE PortalId = @PORTALID)
BBStore_OrderProductOption: SELECT * FROM BBStore_OrderProductOption WHERE OrderProductID IN (SELECT OrderProductId FROM BBSTore_OrderProduct INNER JOIN BBStore_Order ON BBStore_OrderProduct.OrderId = BBStore_Order.OrderID WHERE BBSTore_Order.PortalId = @PORTALID)

BBStore_ProductGroup: SELECT * FROM BBStore_ProductGroup WHERE PortalId = @PORTALID
BBStore_ProductGroupFeature: SELECT * FROM BBStore_ProductGroupFeature WHERE ProductGroupID IN (SELECT ProductGroupID FROM BBStore_ProductGroup WHERE PortalId = @PORTALID)
BBStore_ProductGroupLang: SELECT * FROM BBStore_ProductGroupLang WHERE ProductGroupID IN (SELECT ProductGroupID FROM BBStore_ProductGroup WHERE PortalId = @PORTALID)
BBStore_ProductGroupListItem: SELECT * FROM BBStore_ProductGroupListItem WHERE ProductGroupID IN (SELECT ProductGroupID FROM BBStore_ProductGroup WHERE PortalId = @PORTALID)
BBStore_ProductInGroup: SELECT * FROM BBStore_ProductInGroup WHERE ProductGroupID IN (SELECT ProductGroupID FROM BBStore_ProductGroup WHERE PortalId = @PORTALID)
BBStore_ProductTemplate: SELECT * FROM BBStore_ProductTemplate WHERE PortalID = @PORTALID

BBStore_SimpleProduct : SELECT * FROM BBStore_SimpleProduct WHERE PortalId = @PORTALID
BBStore_SimpleProductLang : SELECT * FROM BBStore_SimpleProductLang WHERE SimpleProductId IN (SELECT SimpleProductID FROM BBStore_SimpleProduct WHERE PortalId = @PORTALID)
BBStore_StaticFilter: SELECT * FROM BBStore_StaticFilter WHERE PortalID = @PORTALID

BBStore_SubscriberPaymentProvider :SELECT * FROM BBStore_SubscriberPaymentProvider WHERE PortalId = @PORTALID

