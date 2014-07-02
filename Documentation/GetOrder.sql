DECLARE @OrderNo VARCHAR(10)
DECLARE @PortalId INT

SET @OrderNo = '00039'
SET PortalId = 22

--select OrderNo, OrderTime,Comment,Currency,Total, CASE PaymentProviderID WHEN 1 THEN 'Vorkasse' ELSE 'Nachnahme' END as Zahlart from bbstore_order where portalid=@PortalId order by orderno 
select OrderNo, OrderTime,Comment,Currency,Total, CASE PaymentProviderID WHEN 1 THEN 'Vorkasse' ELSE 'Nachnahme' END as Zahlart from bbstore_order where portalid=@PortalId AND OrderNo = @OrderNo
SELECT DISTINCT Case oa.Addresstype when 1 THEN 'Rechnungsadresse' ELSE 'Versandadresse' END as Adresstyp,oa.Company,oa.Prefix, oa.Firstname,oa.Middlename,oa.lastname, oa.street, oa.unit, oa.Region, oa.Postalcode,oa.City, oa.Country, oa.Email, oa.telephone From bbstore_OrderAddress oa inner JOIN bbstore_order o on o.OrderId = oa.OrderId WHERE o.OrderNo = @OrderNo
SELECT op.itemno, op.quantity,op.name, op.unitcost, op.TaxPercent, opo.* from bbstore_orderproduct op inner JOIN bbstore_order o on o.OrderId = op.OrderId LEFT OUTER JOIN bbstore_OrderProductOption opo ON op.OrderProductId = opo.OrderProductId WHERE o.OrderNo = @OrderNo
SELECT oap.Quantity, oap.name, oap.UnitCost, oap.TaxPercent, oap.Area from bbstore_orderadditionalcost oap inner JOIN bbstore_order o on o.OrderId = oap.OrderId WHERE o.OrderNo = @OrderNo
