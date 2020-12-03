/**************************************************************************************
Trade Control
Network Demo data extension script
Compatibility: >= 3.27.1 

Date: 25 April 2020
Author: IAM

Trade Control by Trade Control Ltd is licensed under GNU General Public License v3.0. 

You may obtain a copy of the License at

	https://www.gnu.org/licenses/gpl-3.0.en.html

***********************************************************************************/
USE tcTHEBUS;
go

SET NOCOUNT, XACT_ABORT ON;

DECLARE 
	@MaxOrders smallint,
	@ProdCounter smallint, 
	@AccountCode nvarchar(10),
	@AddressCode nvarchar(15),
	@AccountName nvarchar(255),
	@Quantity int, 
	@ActivityCode nvarchar(50), 
	@ActionOn datetime,
	@InvoiceTypeCode smallint, 
	@InvoiceNumber nvarchar(20),
	@InvoicedOn datetime,
	@Id smallint,
	@AreaCode nvarchar(50),
	@IndustrySector nvarchar(50),
	@UserId NVARCHAR(10),
	@TaskCode NVARCHAR(20),
	@ParentTaskCode NVARCHAR(20), 
	@ToTaskCode NVARCHAR(20),
	@CashAccountCode nvarchar(10);

DECLARE @tbAreas TABLE (AreaId smallint, AreaCode nvarchar(10));
DECLARE @tbSectors TABLE (SectorId smallint, IndustrySector nvarchar(50));

BEGIN TRY

	IF NOT EXISTS (SELECT * from App.vwVersion WHERE SQLDataVersion >= 3.27)
		THROW 50000, 'Version incompatible with script', 1; 

	EXEC App.proc_DemoBom @CreateOrders = 1, @InvoiceOrders = 0, @PayInvoices = 0;

	INSERT INTO Activity.tbActivity (ActivityCode, TaskStatusCode, ActivityDescription, UnitOfMeasure, CashCode, UnitCharge, Printed, RegisterName)
	VALUES ('M/00/70/01', 1, 'PIGEON HOLE SHELF ASSEMBLY WHITE', 'each', '103', 18.3240, 1, 'Sales Order')
	, ('M/100/70/01', 1, 'PIGEON HOLE SUB SHELF WHITE', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/101/70/01', 1, 'PIGEON HOLE BACK DIVIDER', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/97/70/01', 1, 'SHELF DIVIDER (WIDE FOOT)', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/99/70/01', 1, 'SHELF DIVIDER (NARROW FOOT)', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('PC/997', 1, 'CALIBRE 303EP WHITE UL94-V2', 'kilo', '200', 2.6500, 1, 'Purchase Order')
	;
	INSERT INTO Activity.tbAttribute (ActivityCode, Attribute, PrintOrder, AttributeTypeCode, DefaultText)
	VALUES ('M/00/70/01', 'Colour', 20, 0, 'WHITE')
	, ('M/00/70/01', 'Colour Number', 10, 0, '-')
	, ('M/00/70/01', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/00/70/01', 'Drawing Issue', 40, 0, '1')
	, ('M/00/70/01', 'Drawing Number', 30, 0, '321554')
	, ('M/00/70/01', 'Label Type', 70, 0, 'Assembly Card')
	, ('M/00/70/01', 'Mould Tool Specification', 110, 1, NULL)
	, ('M/00/70/01', 'Pack Type', 60, 0, 'Despatched')
	, ('M/00/70/01', 'Quantity/Box', 80, 0, '101')
	, ('M/100/70/01', 'Cavities', 170, 0, '1')
	, ('M/100/70/01', 'Colour', 20, 0, 'WHITE')
	, ('M/100/70/01', 'Colour Number', 10, 0, '-')
	, ('M/100/70/01', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/100/70/01', 'Drawing Issue', 40, 0, '1')
	, ('M/100/70/01', 'Drawing Number', 30, 0, '321554-01')
	, ('M/100/70/01', 'Impressions', 180, 0, '1')
	, ('M/100/70/01', 'Label Type', 70, 0, 'Route Card')
	, ('M/100/70/01', 'Location', 150, 0, 'STORES')
	, ('M/100/70/01', 'Pack Type', 60, 0, 'Assembled')
	, ('M/100/70/01', 'Part Weight', 160, 0, '175g')
	, ('M/100/70/01', 'Quantity/Box', 80, 0, '101')
	, ('M/100/70/01', 'Tool Number', 190, 0, '1437')
	, ('M/101/70/01', 'Cavities', 170, 0, '2')
	, ('M/101/70/01', 'Colour', 20, 0, 'WHITE')
	, ('M/101/70/01', 'Colour Number', 10, 0, '-')
	, ('M/101/70/01', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/101/70/01', 'Drawing Issue', 40, 0, '1')
	, ('M/101/70/01', 'Drawing Number', 30, 0, '321554-02')
	, ('M/101/70/01', 'Impressions', 180, 0, '2')
	, ('M/101/70/01', 'Label Type', 70, 0, 'Route Card')
	, ('M/101/70/01', 'Location', 150, 0, 'STORES')
	, ('M/101/70/01', 'Pack Type', 60, 0, 'Assembled')
	, ('M/101/70/01', 'Part Weight', 160, 0, '61g')
	, ('M/101/70/01', 'Quantity/Box', 80, 0, '101')
	, ('M/101/70/01', 'Tool Number', 190, 0, '1439')
	, ('M/97/70/01', 'Cavities', 170, 0, '4')
	, ('M/97/70/01', 'Colour', 20, 0, 'WHITE')
	, ('M/97/70/01', 'Colour Number', 10, 0, '-')
	, ('M/97/70/01', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/97/70/01', 'Drawing Issue', 40, 0, '1')
	, ('M/97/70/01', 'Drawing Number', 30, 0, '321554A')
	, ('M/97/70/01', 'Impressions', 180, 0, '4')
	, ('M/97/70/01', 'Label Type', 70, 0, 'Route Card')
	, ('M/97/70/01', 'Location', 150, 0, 'STORES')
	, ('M/97/70/01', 'Pack Type', 60, 0, 'Assembled')
	, ('M/97/70/01', 'Part Weight', 160, 0, '171g')
	, ('M/97/70/01', 'Quantity/Box', 80, 0, '101')
	, ('M/97/70/01', 'Tool Number', 190, 0, '1440')
	, ('M/99/70/01', 'Cavities', 170, 0, '1')
	, ('M/99/70/01', 'Colour', 20, 0, 'WHITE')
	, ('M/99/70/01', 'Colour Number', 10, 0, '-')
	, ('M/99/70/01', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/99/70/01', 'Drawing Issue', 40, 0, '1')
	, ('M/99/70/01', 'Drawing Number', 30, 0, '321554A')
	, ('M/99/70/01', 'Impressions', 180, 0, '1')
	, ('M/99/70/01', 'Label Type', 70, 0, 'Route Card')
	, ('M/99/70/01', 'Location', 150, 0, 'STORES')
	, ('M/99/70/01', 'Pack Type', 60, 0, 'Assembled')
	, ('M/99/70/01', 'Part Weight', 160, 0, '171g')
	, ('M/99/70/01', 'Quantity/Box', 80, 0, '101')
	, ('M/99/70/01', 'Tool Number', 190, 0, '1441')
	, ('PC/997', 'Colour', 50, 0, 'WHITE')
	, ('PC/997', 'Grade', 20, 0, '303EP')
	, ('PC/997', 'Location', 60, 0, 'R2123-9')
	, ('PC/997', 'Material Type', 10, 0, 'PC')
	, ('PC/997', 'Name', 30, 0, 'Calibre')
	, ('PC/997', 'SG', 40, 0, '1.21')
	;
	INSERT INTO Activity.tbOp (ActivityCode, OperationNumber, SyncTypeCode, Operation, Duration, OffsetDays)
	VALUES ('M/00/70/01', 10, 0, 'ASSEMBLE', 0.5, 3)
	, ('M/00/70/01', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/00/70/01', 30, 0, 'PACK', 0, 1)
	, ('M/00/70/01', 40, 2, 'DELIVER', 0, 1)
	, ('M/100/70/01', 10, 0, 'MOULD', 10, 2)
	, ('M/100/70/01', 20, 1, 'INSERTS', 0, 0)
	, ('M/100/70/01', 30, 0, 'QUALITY CHECK', 0, 0)
	, ('M/101/70/01', 10, 0, 'MOULD', 10, 0)
	, ('M/101/70/01', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/97/70/01', 10, 0, 'MOULD', 10, 2)
	, ('M/97/70/01', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/99/70/01', 10, 0, 'MOULD', 0, 2)
	, ('M/99/70/01', 20, 0, 'QUALITY CHECK', 0, 0)
	;
	INSERT INTO Activity.tbFlow (ParentCode, StepNumber, ChildCode, SyncTypeCode, OffsetDays, UsedOnQuantity)
	VALUES ('M/00/70/01', 10, 'M/100/70/01', 1, 0, 8)
	, ('M/00/70/01', 20, 'M/101/70/01', 1, 0, 4)
	, ('M/00/70/01', 30, 'M/97/70/01', 1, 0, 3)
	, ('M/00/70/01', 40, 'M/99/70/01', 0, 0, 2)
	, ('M/00/70/01', 50, 'BOX/41', 1, 0, 1)
	, ('M/00/70/01', 60, 'PALLET/01', 1, 0, 0.01)
	, ('M/00/70/01', 70, 'DELIVERY', 2, 1, 0)
	, ('M/100/70/01', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/100/70/01', 20, 'PC/997', 1, 0, 0.175)
	, ('M/101/70/01', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/101/70/01', 20, 'PC/997', 1, 0, 0.061)
	, ('M/97/70/01', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/97/70/01', 20, 'PC/997', 1, 0, 0.172)
	, ('M/99/70/01', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/99/70/01', 20, 'PC/997', 1, 0, 0.171)
	, ('M/100/70/01', 30, 'INSERT/09', 1, 0, 2)
	;

	/**********************************************************************************************************/

	INSERT INTO Activity.tbActivity (ActivityCode, TaskStatusCode, ActivityDescription, UnitOfMeasure, CashCode, UnitCharge, Printed, RegisterName)
	VALUES ('M/00/70/02', 1, 'PIGEON HOLE SHELF ASSEMBLY BLUE', 'each', '103', 17.500, 1, 'Sales Order')
	, ('M/100/70/02', 1, 'PIGEON HOLE SUB SHELF BLUE', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/101/70/02', 1, 'PIGEON HOLE BACK DIVIDER', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/97/70/02', 1, 'SHELF DIVIDER (WIDE FOOT)', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/99/70/02', 1, 'SHELF DIVIDER (NARROW FOOT)', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('PC/998', 1, 'CALIBRE 303EP BLUE UL94-V2', 'kilo', '200', 2.5500, 1, 'Purchase Order')
	;
	INSERT INTO Activity.tbAttribute (ActivityCode, Attribute, PrintOrder, AttributeTypeCode, DefaultText)
	VALUES ('M/00/70/02', 'Colour', 20, 0, 'BLUE')
	, ('M/00/70/02', 'Colour Number', 10, 0, '-')
	, ('M/00/70/02', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/00/70/02', 'Drawing Issue', 40, 0, '1')
	, ('M/00/70/02', 'Drawing Number', 30, 0, '321554')
	, ('M/00/70/02', 'Label Type', 70, 0, 'Assembly Card')
	, ('M/00/70/02', 'Mould Tool Specification', 110, 1, NULL)
	, ('M/00/70/02', 'Pack Type', 60, 0, 'Despatched')
	, ('M/00/70/02', 'Quantity/Box', 80, 0, '102')
	, ('M/100/70/02', 'Cavities', 170, 0, '1')
	, ('M/100/70/02', 'Colour', 20, 0, 'BLUE')
	, ('M/100/70/02', 'Colour Number', 10, 0, '-')
	, ('M/100/70/02', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/100/70/02', 'Drawing Issue', 40, 0, '1')
	, ('M/100/70/02', 'Drawing Number', 30, 0, '321554-01')
	, ('M/100/70/02', 'Impressions', 180, 0, '1')
	, ('M/100/70/02', 'Label Type', 70, 0, 'Route Card')
	, ('M/100/70/02', 'Location', 150, 0, 'STORES')
	, ('M/100/70/02', 'Pack Type', 60, 0, 'Assembled')
	, ('M/100/70/02', 'Part Weight', 160, 0, '175g')
	, ('M/100/70/02', 'Quantity/Box', 80, 0, '102')
	, ('M/100/70/02', 'Tool Number', 190, 0, '1437')
	, ('M/101/70/02', 'Cavities', 170, 0, '2')
	, ('M/101/70/02', 'Colour', 20, 0, 'BLUE')
	, ('M/101/70/02', 'Colour Number', 10, 0, '-')
	, ('M/101/70/02', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/101/70/02', 'Drawing Issue', 40, 0, '1')
	, ('M/101/70/02', 'Drawing Number', 30, 0, '321554-02')
	, ('M/101/70/02', 'Impressions', 180, 0, '2')
	, ('M/101/70/02', 'Label Type', 70, 0, 'Route Card')
	, ('M/101/70/02', 'Location', 150, 0, 'STORES')
	, ('M/101/70/02', 'Pack Type', 60, 0, 'Assembled')
	, ('M/101/70/02', 'Part Weight', 160, 0, '61g')
	, ('M/101/70/02', 'Quantity/Box', 80, 0, '102')
	, ('M/101/70/02', 'Tool Number', 190, 0, '1439')
	, ('M/97/70/02', 'Cavities', 170, 0, '4')
	, ('M/97/70/02', 'Colour', 20, 0, 'BLUE')
	, ('M/97/70/02', 'Colour Number', 10, 0, '-')
	, ('M/97/70/02', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/97/70/02', 'Drawing Issue', 40, 0, '1')
	, ('M/97/70/02', 'Drawing Number', 30, 0, '321554A')
	, ('M/97/70/02', 'Impressions', 180, 0, '4')
	, ('M/97/70/02', 'Label Type', 70, 0, 'Route Card')
	, ('M/97/70/02', 'Location', 150, 0, 'STORES')
	, ('M/97/70/02', 'Pack Type', 60, 0, 'Assembled')
	, ('M/97/70/02', 'Part Weight', 160, 0, '171g')
	, ('M/97/70/02', 'Quantity/Box', 80, 0, '102')
	, ('M/97/70/02', 'Tool Number', 190, 0, '1440')
	, ('M/99/70/02', 'Cavities', 170, 0, '1')
	, ('M/99/70/02', 'Colour', 20, 0, 'BLUE')
	, ('M/99/70/02', 'Colour Number', 10, 0, '-')
	, ('M/99/70/02', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/99/70/02', 'Drawing Issue', 40, 0, '1')
	, ('M/99/70/02', 'Drawing Number', 30, 0, '321554A')
	, ('M/99/70/02', 'Impressions', 180, 0, '1')
	, ('M/99/70/02', 'Label Type', 70, 0, 'Route Card')
	, ('M/99/70/02', 'Location', 150, 0, 'STORES')
	, ('M/99/70/02', 'Pack Type', 60, 0, 'Assembled')
	, ('M/99/70/02', 'Part Weight', 160, 0, '171g')
	, ('M/99/70/02', 'Quantity/Box', 80, 0, '102')
	, ('M/99/70/02', 'Tool Number', 190, 0, '1441')
	, ('PC/998', 'Colour', 50, 0, 'BLUE')
	, ('PC/998', 'Grade', 20, 0, '303EP')
	, ('PC/998', 'Location', 60, 0, 'R2123-9')
	, ('PC/998', 'Material Type', 10, 0, 'PC')
	, ('PC/998', 'Name', 30, 0, 'Calibre')
	, ('PC/998', 'SG', 40, 0, '1.21')
	;
	INSERT INTO Activity.tbOp (ActivityCode, OperationNumber, SyncTypeCode, Operation, Duration, OffsetDays)
	VALUES ('M/00/70/02', 10, 0, 'ASSEMBLE', 0.5, 3)
	, ('M/00/70/02', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/00/70/02', 30, 0, 'PACK', 0, 1)
	, ('M/00/70/02', 40, 2, 'DELIVER', 0, 1)
	, ('M/100/70/02', 10, 0, 'MOULD', 10, 2)
	, ('M/100/70/02', 20, 1, 'INSERTS', 0, 0)
	, ('M/100/70/02', 30, 0, 'QUALITY CHECK', 0, 0)
	, ('M/101/70/02', 10, 0, 'MOULD', 10, 0)
	, ('M/101/70/02', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/97/70/02', 10, 0, 'MOULD', 10, 2)
	, ('M/97/70/02', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/99/70/02', 10, 0, 'MOULD', 0, 2)
	, ('M/99/70/02', 20, 0, 'QUALITY CHECK', 0, 0)
	;
	INSERT INTO Activity.tbFlow (ParentCode, StepNumber, ChildCode, SyncTypeCode, OffsetDays, UsedOnQuantity)
	VALUES ('M/00/70/02', 10, 'M/100/70/02', 1, 0, 8)
	, ('M/00/70/02', 20, 'M/101/70/02', 1, 0, 4)
	, ('M/00/70/02', 30, 'M/97/70/02', 1, 0, 3)
	, ('M/00/70/02', 40, 'M/99/70/02', 0, 0, 2)
	, ('M/00/70/02', 50, 'BOX/41', 1, 0, 1)
	, ('M/00/70/02', 60, 'PALLET/01', 1, 0, 0.01)
	, ('M/00/70/02', 70, 'DELIVERY', 2, 1, 0)
	, ('M/100/70/02', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/100/70/02', 20, 'PC/998', 1, 0, 0.175)
	, ('M/101/70/02', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/101/70/02', 20, 'PC/998', 1, 0, 0.061)
	, ('M/97/70/02', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/97/70/02', 20, 'PC/998', 1, 0, 0.172)
	, ('M/99/70/02', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/99/70/02', 20, 'PC/998', 1, 0, 0.171)
	, ('M/100/70/02', 30, 'INSERT/09', 1, 0, 2)
	;

	/***********************************************************************************/

	INSERT INTO Activity.tbActivity (ActivityCode, TaskStatusCode, ActivityDescription, UnitOfMeasure, CashCode, UnitCharge, Printed, RegisterName)
	VALUES ('M/00/70/03', 1, 'PIGEON HOLE SHELF ASSEMBLY GREEN', 'each', '103', 14.2240, 1, 'Sales Order')
	, ('M/100/70/03', 1, 'PIGEON HOLE SUB SHELF GREEN', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/101/70/03', 1, 'PIGEON HOLE BACK DIVIDER', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/97/70/03', 1, 'SHELF DIVIDER (WIDE FOOT)', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/99/70/03', 1, 'SHELF DIVIDER (NARROW FOOT)', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('PC/996', 1, 'CALIBRE 303EP GREEN UL94-V2', 'kilo', '200', 2.000, 1, 'Purchase Order')
	;
	INSERT INTO Activity.tbAttribute (ActivityCode, Attribute, PrintOrder, AttributeTypeCode, DefaultText)
	VALUES ('M/00/70/03', 'Colour', 20, 0, 'GREEN')
	, ('M/00/70/03', 'Colour Number', 10, 0, '-')
	, ('M/00/70/03', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/00/70/03', 'Drawing Issue', 40, 0, '1')
	, ('M/00/70/03', 'Drawing Number', 30, 0, '321554')
	, ('M/00/70/03', 'Label Type', 70, 0, 'Assembly Card')
	, ('M/00/70/03', 'Mould Tool Specification', 110, 1, NULL)
	, ('M/00/70/03', 'Pack Type', 60, 0, 'Despatched')
	, ('M/00/70/03', 'Quantity/Box', 80, 0, '103')
	, ('M/100/70/03', 'Cavities', 170, 0, '1')
	, ('M/100/70/03', 'Colour', 20, 0, 'GREEN')
	, ('M/100/70/03', 'Colour Number', 10, 0, '-')
	, ('M/100/70/03', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/100/70/03', 'Drawing Issue', 40, 0, '1')
	, ('M/100/70/03', 'Drawing Number', 30, 0, '321554-03')
	, ('M/100/70/03', 'Impressions', 180, 0, '1')
	, ('M/100/70/03', 'Label Type', 70, 0, 'Route Card')
	, ('M/100/70/03', 'Location', 150, 0, 'STORES')
	, ('M/100/70/03', 'Pack Type', 60, 0, 'Assembled')
	, ('M/100/70/03', 'Part Weight', 160, 0, '175g')
	, ('M/100/70/03', 'Quantity/Box', 80, 0, '103')
	, ('M/100/70/03', 'Tool Number', 190, 0, '1437')
	, ('M/101/70/03', 'Cavities', 170, 0, '2')
	, ('M/101/70/03', 'Colour', 20, 0, 'GREEN')
	, ('M/101/70/03', 'Colour Number', 10, 0, '-')
	, ('M/101/70/03', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/101/70/03', 'Drawing Issue', 40, 0, '1')
	, ('M/101/70/03', 'Drawing Number', 30, 0, '321554-02')
	, ('M/101/70/03', 'Impressions', 180, 0, '2')
	, ('M/101/70/03', 'Label Type', 70, 0, 'Route Card')
	, ('M/101/70/03', 'Location', 150, 0, 'STORES')
	, ('M/101/70/03', 'Pack Type', 60, 0, 'Assembled')
	, ('M/101/70/03', 'Part Weight', 160, 0, '61g')
	, ('M/101/70/03', 'Quantity/Box', 80, 0, '103')
	, ('M/101/70/03', 'Tool Number', 190, 0, '1439')
	, ('M/97/70/03', 'Cavities', 170, 0, '4')
	, ('M/97/70/03', 'Colour', 20, 0, 'GREEN')
	, ('M/97/70/03', 'Colour Number', 10, 0, '-')
	, ('M/97/70/03', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/97/70/03', 'Drawing Issue', 40, 0, '1')
	, ('M/97/70/03', 'Drawing Number', 30, 0, '321554A')
	, ('M/97/70/03', 'Impressions', 180, 0, '4')
	, ('M/97/70/03', 'Label Type', 70, 0, 'Route Card')
	, ('M/97/70/03', 'Location', 150, 0, 'STORES')
	, ('M/97/70/03', 'Pack Type', 60, 0, 'Assembled')
	, ('M/97/70/03', 'Part Weight', 160, 0, '171g')
	, ('M/97/70/03', 'Quantity/Box', 80, 0, '103')
	, ('M/97/70/03', 'Tool Number', 190, 0, '1440')
	, ('M/99/70/03', 'Cavities', 170, 0, '1')
	, ('M/99/70/03', 'Colour', 20, 0, 'GREEN')
	, ('M/99/70/03', 'Colour Number', 10, 0, '-')
	, ('M/99/70/03', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/99/70/03', 'Drawing Issue', 40, 0, '1')
	, ('M/99/70/03', 'Drawing Number', 30, 0, '321554A')
	, ('M/99/70/03', 'Impressions', 180, 0, '1')
	, ('M/99/70/03', 'Label Type', 70, 0, 'Route Card')
	, ('M/99/70/03', 'Location', 150, 0, 'STORES')
	, ('M/99/70/03', 'Pack Type', 60, 0, 'Assembled')
	, ('M/99/70/03', 'Part Weight', 160, 0, '171g')
	, ('M/99/70/03', 'Quantity/Box', 80, 0, '103')
	, ('M/99/70/03', 'Tool Number', 190, 0, '1441')
	, ('PC/996', 'Colour', 50, 0, 'GREEN')
	, ('PC/996', 'Grade', 20, 0, '303EP')
	, ('PC/996', 'Location', 60, 0, 'R2123-9')
	, ('PC/996', 'Material Type', 10, 0, 'PC')
	, ('PC/996', 'Name', 30, 0, 'Calibre')
	, ('PC/996', 'SG', 40, 0, '1.21')
	;
	INSERT INTO Activity.tbOp (ActivityCode, OperationNumber, SyncTypeCode, Operation, Duration, OffsetDays)
	VALUES ('M/00/70/03', 10, 0, 'ASSEMBLE', 0.5, 3)
	, ('M/00/70/03', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/00/70/03', 30, 0, 'PACK', 0, 1)
	, ('M/00/70/03', 40, 2, 'DELIVER', 0, 1)
	, ('M/100/70/03', 10, 0, 'MOULD', 10, 2)
	, ('M/100/70/03', 20, 1, 'INSERTS', 0, 0)
	, ('M/100/70/03', 30, 0, 'QUALITY CHECK', 0, 0)
	, ('M/101/70/03', 10, 0, 'MOULD', 10, 0)
	, ('M/101/70/03', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/97/70/03', 10, 0, 'MOULD', 10, 2)
	, ('M/97/70/03', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/99/70/03', 10, 0, 'MOULD', 0, 2)
	, ('M/99/70/03', 20, 0, 'QUALITY CHECK', 0, 0)
	;
	INSERT INTO Activity.tbFlow (ParentCode, StepNumber, ChildCode, SyncTypeCode, OffsetDays, UsedOnQuantity)
	VALUES ('M/00/70/03', 10, 'M/100/70/03', 1, 0, 8)
	, ('M/00/70/03', 20, 'M/101/70/03', 1, 0, 4)
	, ('M/00/70/03', 30, 'M/97/70/03', 1, 0, 3)
	, ('M/00/70/03', 40, 'M/99/70/03', 0, 0, 2)
	, ('M/00/70/03', 50, 'BOX/41', 1, 0, 1)
	, ('M/00/70/03', 60, 'PALLET/01', 1, 0, 0.01)
	, ('M/00/70/03', 70, 'DELIVERY', 2, 1, 0)
	, ('M/100/70/03', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/100/70/03', 20, 'PC/996', 1, 0, 0.175)
	, ('M/101/70/03', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/101/70/03', 20, 'PC/996', 1, 0, 0.061)
	, ('M/97/70/03', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/97/70/03', 20, 'PC/996', 1, 0, 0.172)
	, ('M/99/70/03', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/99/70/03', 20, 'PC/996', 1, 0, 0.171)
	, ('M/100/70/03', 30, 'INSERT/09', 1, 0, 2)
	;

	/**********************************************************************************************************/

	INSERT INTO Activity.tbActivity (ActivityCode, TaskStatusCode, ActivityDescription, UnitOfMeasure, CashCode, UnitCharge, Printed, RegisterName)
	VALUES ('M/00/70/04', 1, 'PIGEON HOLE SHELF ASSEMBLY GREY', 'each', '103', 12.10, 1, 'Sales Order')
	, ('M/100/70/04', 1, 'PIGEON HOLE SUB SHELF GREY', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/101/70/04', 1, 'PIGEON HOLE BACK DIVIDER', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/97/70/04', 1, 'SHELF DIVIDER (WIDE FOOT)', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/99/70/04', 1, 'SHELF DIVIDER (NARROW FOOT)', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('PC/001', 1, 'CALIBRE 303EP GREY UL94-V2', 'kilo', '200', 3.1500, 1, 'Purchase Order')
	;
	INSERT INTO Activity.tbAttribute (ActivityCode, Attribute, PrintOrder, AttributeTypeCode, DefaultText)
	VALUES ('M/00/70/04', 'Colour', 20, 0, 'GREY')
	, ('M/00/70/04', 'Colour Number', 10, 0, '-')
	, ('M/00/70/04', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/00/70/04', 'Drawing Issue', 40, 0, '1')
	, ('M/00/70/04', 'Drawing Number', 30, 0, '321554')
	, ('M/00/70/04', 'Label Type', 70, 0, 'Assembly Card')
	, ('M/00/70/04', 'Mould Tool Specification', 110, 1, NULL)
	, ('M/00/70/04', 'Pack Type', 60, 0, 'Despatched')
	, ('M/00/70/04', 'Quantity/Box', 80, 0, '104')
	, ('M/100/70/04', 'Cavities', 170, 0, '1')
	, ('M/100/70/04', 'Colour', 20, 0, 'GREY')
	, ('M/100/70/04', 'Colour Number', 10, 0, '-')
	, ('M/100/70/04', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/100/70/04', 'Drawing Issue', 40, 0, '1')
	, ('M/100/70/04', 'Drawing Number', 30, 0, '321554-04')
	, ('M/100/70/04', 'Impressions', 180, 0, '1')
	, ('M/100/70/04', 'Label Type', 70, 0, 'Route Card')
	, ('M/100/70/04', 'Location', 150, 0, 'STORES')
	, ('M/100/70/04', 'Pack Type', 60, 0, 'Assembled')
	, ('M/100/70/04', 'Part Weight', 160, 0, '175g')
	, ('M/100/70/04', 'Quantity/Box', 80, 0, '104')
	, ('M/100/70/04', 'Tool Number', 190, 0, '1437')
	, ('M/101/70/04', 'Cavities', 170, 0, '2')
	, ('M/101/70/04', 'Colour', 20, 0, 'GREY')
	, ('M/101/70/04', 'Colour Number', 10, 0, '-')
	, ('M/101/70/04', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/101/70/04', 'Drawing Issue', 40, 0, '1')
	, ('M/101/70/04', 'Drawing Number', 30, 0, '321554-05')
	, ('M/101/70/04', 'Impressions', 180, 0, '2')
	, ('M/101/70/04', 'Label Type', 70, 0, 'Route Card')
	, ('M/101/70/04', 'Location', 150, 0, 'STORES')
	, ('M/101/70/04', 'Pack Type', 60, 0, 'Assembled')
	, ('M/101/70/04', 'Part Weight', 160, 0, '61g')
	, ('M/101/70/04', 'Quantity/Box', 80, 0, '104')
	, ('M/101/70/04', 'Tool Number', 190, 0, '1439')
	, ('M/97/70/04', 'Cavities', 170, 0, '4')
	, ('M/97/70/04', 'Colour', 20, 0, 'GREY')
	, ('M/97/70/04', 'Colour Number', 10, 0, '-')
	, ('M/97/70/04', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/97/70/04', 'Drawing Issue', 40, 0, '1')
	, ('M/97/70/04', 'Drawing Number', 30, 0, '321554A')
	, ('M/97/70/04', 'Impressions', 180, 0, '4')
	, ('M/97/70/04', 'Label Type', 70, 0, 'Route Card')
	, ('M/97/70/04', 'Location', 150, 0, 'STORES')
	, ('M/97/70/04', 'Pack Type', 60, 0, 'Assembled')
	, ('M/97/70/04', 'Part Weight', 160, 0, '171g')
	, ('M/97/70/04', 'Quantity/Box', 80, 0, '104')
	, ('M/97/70/04', 'Tool Number', 190, 0, '1440')
	, ('M/99/70/04', 'Cavities', 170, 0, '1')
	, ('M/99/70/04', 'Colour', 20, 0, 'GREY')
	, ('M/99/70/04', 'Colour Number', 10, 0, '-')
	, ('M/99/70/04', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/99/70/04', 'Drawing Issue', 40, 0, '1')
	, ('M/99/70/04', 'Drawing Number', 30, 0, '321554A')
	, ('M/99/70/04', 'Impressions', 180, 0, '1')
	, ('M/99/70/04', 'Label Type', 70, 0, 'Route Card')
	, ('M/99/70/04', 'Location', 150, 0, 'STORES')
	, ('M/99/70/04', 'Pack Type', 60, 0, 'Assembled')
	, ('M/99/70/04', 'Part Weight', 160, 0, '171g')
	, ('M/99/70/04', 'Quantity/Box', 80, 0, '104')
	, ('M/99/70/04', 'Tool Number', 190, 0, '1441')
	, ('PC/001', 'Colour', 50, 0, 'GREY')
	, ('PC/001', 'Grade', 20, 0, '303EP')
	, ('PC/001', 'Location', 60, 0, 'R2123-9')
	, ('PC/001', 'Material Type', 10, 0, 'PC')
	, ('PC/001', 'Name', 30, 0, 'Calibre')
	, ('PC/001', 'SG', 40, 0, '1.21')
	;
	INSERT INTO Activity.tbOp (ActivityCode, OperationNumber, SyncTypeCode, Operation, Duration, OffsetDays)
	VALUES ('M/00/70/04', 10, 0, 'ASSEMBLE', 0.5, 3)
	, ('M/00/70/04', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/00/70/04', 30, 0, 'PACK', 0, 1)
	, ('M/00/70/04', 40, 2, 'DELIVER', 0, 1)
	, ('M/100/70/04', 10, 0, 'MOULD', 10, 2)
	, ('M/100/70/04', 20, 1, 'INSERTS', 0, 0)
	, ('M/100/70/04', 30, 0, 'QUALITY CHECK', 0, 0)
	, ('M/101/70/04', 10, 0, 'MOULD', 10, 0)
	, ('M/101/70/04', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/97/70/04', 10, 0, 'MOULD', 10, 2)
	, ('M/97/70/04', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/99/70/04', 10, 0, 'MOULD', 0, 2)
	, ('M/99/70/04', 20, 0, 'QUALITY CHECK', 0, 0)
	;
	INSERT INTO Activity.tbFlow (ParentCode, StepNumber, ChildCode, SyncTypeCode, OffsetDays, UsedOnQuantity)
	VALUES ('M/00/70/04', 10, 'M/100/70/04', 1, 0, 8)
	, ('M/00/70/04', 20, 'M/101/70/04', 1, 0, 4)
	, ('M/00/70/04', 30, 'M/97/70/04', 1, 0, 3)
	, ('M/00/70/04', 40, 'M/99/70/04', 0, 0, 2)
	, ('M/00/70/04', 50, 'BOX/41', 1, 0, 1)
	, ('M/00/70/04', 60, 'PALLET/01', 1, 0, 0.01)
	, ('M/00/70/04', 70, 'DELIVERY', 2, 1, 0)
	, ('M/100/70/04', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/100/70/04', 20, 'PC/001', 1, 0, 0.175)
	, ('M/101/70/04', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/101/70/04', 20, 'PC/001', 1, 0, 0.061)
	, ('M/97/70/04', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/97/70/04', 20, 'PC/001', 1, 0, 0.172)
	, ('M/99/70/04', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/99/70/04', 20, 'PC/001', 1, 0, 0.171)
	, ('M/100/70/04', 30, 'INSERT/09', 1, 0, 2)
	;

	/**********************************************************************************************************/

	INSERT INTO Activity.tbActivity (ActivityCode, TaskStatusCode, ActivityDescription, UnitOfMeasure, CashCode, UnitCharge, Printed, RegisterName)
	VALUES ('M/00/70/05', 1, 'PIGEON HOLE SHELF ASSEMBLY RED', 'each', '103', 18.0200, 1, 'Sales Order')
	, ('M/100/70/05', 1, 'PIGEON HOLE SUB SHELF RED', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/101/70/05', 1, 'PIGEON HOLE BACK DIVIDER', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/97/70/05', 1, 'SHELF DIVIDER (WIDE FOOT)', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/99/70/05', 1, 'SHELF DIVIDER (NARROW FOOT)', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('PC/005', 1, 'CALIBRE 303EP RED UL94-V2', 'kilo', '200', 3.0000, 1, 'Purchase Order')
	;
	INSERT INTO Activity.tbAttribute (ActivityCode, Attribute, PrintOrder, AttributeTypeCode, DefaultText)
	VALUES ('M/00/70/05', 'Colour', 20, 0, 'RED')
	, ('M/00/70/05', 'Colour Number', 10, 0, '-')
	, ('M/00/70/05', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/00/70/05', 'Drawing Issue', 40, 0, '1')
	, ('M/00/70/05', 'Drawing Number', 30, 0, '321554')
	, ('M/00/70/05', 'Label Type', 70, 0, 'Assembly Card')
	, ('M/00/70/05', 'Mould Tool Specification', 110, 1, NULL)
	, ('M/00/70/05', 'Pack Type', 60, 0, 'Despatched')
	, ('M/00/70/05', 'Quantity/Box', 80, 0, '105')
	, ('M/100/70/05', 'Cavities', 170, 0, '1')
	, ('M/100/70/05', 'Colour', 20, 0, 'RED')
	, ('M/100/70/05', 'Colour Number', 10, 0, '-')
	, ('M/100/70/05', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/100/70/05', 'Drawing Issue', 40, 0, '1')
	, ('M/100/70/05', 'Drawing Number', 30, 0, '321554-04')
	, ('M/100/70/05', 'Impressions', 180, 0, '1')
	, ('M/100/70/05', 'Label Type', 70, 0, 'Route Card')
	, ('M/100/70/05', 'Location', 150, 0, 'STORES')
	, ('M/100/70/05', 'Pack Type', 60, 0, 'Assembled')
	, ('M/100/70/05', 'Part Weight', 160, 0, '175g')
	, ('M/100/70/05', 'Quantity/Box', 80, 0, '105')
	, ('M/100/70/05', 'Tool Number', 190, 0, '1437')
	, ('M/101/70/05', 'Cavities', 170, 0, '2')
	, ('M/101/70/05', 'Colour', 20, 0, 'RED')
	, ('M/101/70/05', 'Colour Number', 10, 0, '-')
	, ('M/101/70/05', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/101/70/05', 'Drawing Issue', 40, 0, '1')
	, ('M/101/70/05', 'Drawing Number', 30, 0, '321554-05')
	, ('M/101/70/05', 'Impressions', 180, 0, '2')
	, ('M/101/70/05', 'Label Type', 70, 0, 'Route Card')
	, ('M/101/70/05', 'Location', 150, 0, 'STORES')
	, ('M/101/70/05', 'Pack Type', 60, 0, 'Assembled')
	, ('M/101/70/05', 'Part Weight', 160, 0, '61g')
	, ('M/101/70/05', 'Quantity/Box', 80, 0, '105')
	, ('M/101/70/05', 'Tool Number', 190, 0, '1439')
	, ('M/97/70/05', 'Cavities', 170, 0, '4')
	, ('M/97/70/05', 'Colour', 20, 0, 'RED')
	, ('M/97/70/05', 'Colour Number', 10, 0, '-')
	, ('M/97/70/05', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/97/70/05', 'Drawing Issue', 40, 0, '1')
	, ('M/97/70/05', 'Drawing Number', 30, 0, '321554A')
	, ('M/97/70/05', 'Impressions', 180, 0, '4')
	, ('M/97/70/05', 'Label Type', 70, 0, 'Route Card')
	, ('M/97/70/05', 'Location', 150, 0, 'STORES')
	, ('M/97/70/05', 'Pack Type', 60, 0, 'Assembled')
	, ('M/97/70/05', 'Part Weight', 160, 0, '171g')
	, ('M/97/70/05', 'Quantity/Box', 80, 0, '105')
	, ('M/97/70/05', 'Tool Number', 190, 0, '1440')
	, ('M/99/70/05', 'Cavities', 170, 0, '1')
	, ('M/99/70/05', 'Colour', 20, 0, 'RED')
	, ('M/99/70/05', 'Colour Number', 10, 0, '-')
	, ('M/99/70/05', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/99/70/05', 'Drawing Issue', 40, 0, '1')
	, ('M/99/70/05', 'Drawing Number', 30, 0, '321554A')
	, ('M/99/70/05', 'Impressions', 180, 0, '1')
	, ('M/99/70/05', 'Label Type', 70, 0, 'Route Card')
	, ('M/99/70/05', 'Location', 150, 0, 'STORES')
	, ('M/99/70/05', 'Pack Type', 60, 0, 'Assembled')
	, ('M/99/70/05', 'Part Weight', 160, 0, '171g')
	, ('M/99/70/05', 'Quantity/Box', 80, 0, '105')
	, ('M/99/70/05', 'Tool Number', 190, 0, '1441')
	, ('PC/005', 'Colour', 50, 0, 'RED')
	, ('PC/005', 'Grade', 20, 0, '303EP')
	, ('PC/005', 'Location', 60, 0, 'R2123-9')
	, ('PC/005', 'Material Type', 10, 0, 'PC')
	, ('PC/005', 'Name', 30, 0, 'Calibre')
	, ('PC/005', 'SG', 40, 0, '1.21')
	;
	INSERT INTO Activity.tbOp (ActivityCode, OperationNumber, SyncTypeCode, Operation, Duration, OffsetDays)
	VALUES ('M/00/70/05', 10, 0, 'ASSEMBLE', 0.5, 3)
	, ('M/00/70/05', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/00/70/05', 30, 0, 'PACK', 0, 1)
	, ('M/00/70/05', 40, 2, 'DELIVER', 0, 1)
	, ('M/100/70/05', 10, 0, 'MOULD', 10, 2)
	, ('M/100/70/05', 20, 1, 'INSERTS', 0, 0)
	, ('M/100/70/05', 30, 0, 'QUALITY CHECK', 0, 0)
	, ('M/101/70/05', 10, 0, 'MOULD', 10, 0)
	, ('M/101/70/05', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/97/70/05', 10, 0, 'MOULD', 10, 2)
	, ('M/97/70/05', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/99/70/05', 10, 0, 'MOULD', 0, 2)
	, ('M/99/70/05', 20, 0, 'QUALITY CHECK', 0, 0)
	;
	INSERT INTO Activity.tbFlow (ParentCode, StepNumber, ChildCode, SyncTypeCode, OffsetDays, UsedOnQuantity)
	VALUES ('M/00/70/05', 10, 'M/100/70/05', 1, 0, 8)
	, ('M/00/70/05', 20, 'M/101/70/05', 1, 0, 4)
	, ('M/00/70/05', 30, 'M/97/70/05', 1, 0, 3)
	, ('M/00/70/05', 40, 'M/99/70/05', 0, 0, 2)
	, ('M/00/70/05', 50, 'BOX/41', 1, 0, 1)
	, ('M/00/70/05', 60, 'PALLET/01', 1, 0, 0.01)
	, ('M/00/70/05', 70, 'DELIVERY', 2, 1, 0)
	, ('M/100/70/05', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/100/70/05', 20, 'PC/005', 1, 0, 0.175)
	, ('M/101/70/05', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/101/70/05', 20, 'PC/005', 1, 0, 0.061)
	, ('M/97/70/05', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/97/70/05', 20, 'PC/005', 1, 0, 0.172)
	, ('M/99/70/05', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/99/70/05', 20, 'PC/005', 1, 0, 0.171)
	, ('M/100/70/05', 30, 'INSERT/09', 1, 0, 2)
	;

	/***********************************************************************************/

	INSERT INTO Activity.tbActivity (ActivityCode, TaskStatusCode, ActivityDescription, UnitOfMeasure, CashCode, UnitCharge, Printed, RegisterName)
	VALUES ('M/00/70/06', 1, 'PIGEON HOLE SHELF ASSEMBLY YELLOW', 'each', '103', 14.530, 1, 'Sales Order')
	, ('M/100/70/06', 1, 'PIGEON HOLE SUB SHELF YELLOW', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/101/70/06', 1, 'PIGEON HOLE BACK DIVIDER', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/97/70/06', 1, 'SHELF DIVIDER (WIDE FOOT)', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('M/99/70/06', 1, 'SHELF DIVIDER (NARROW FOOT)', 'each', NULL, 0.0000, 0, 'Works Order')
	, ('PC/004', 1, 'CALIBRE 303EP YELLOW UL94-V2', 'kilo', '200', 1.8500, 1, 'Purchase Order')
	;
	INSERT INTO Activity.tbAttribute (ActivityCode, Attribute, PrintOrder, AttributeTypeCode, DefaultText)
	VALUES ('M/00/70/06', 'Colour', 20, 0, 'YELLOW')
	, ('M/00/70/06', 'Colour Number', 10, 0, '-')
	, ('M/00/70/06', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/00/70/06', 'Drawing Issue', 40, 0, '1')
	, ('M/00/70/06', 'Drawing Number', 30, 0, '321554')
	, ('M/00/70/06', 'Label Type', 70, 0, 'Assembly Card')
	, ('M/00/70/06', 'Mould Tool Specification', 110, 1, NULL)
	, ('M/00/70/06', 'Pack Type', 60, 0, 'Despatched')
	, ('M/00/70/06', 'Quantity/Box', 80, 0, '103')
	, ('M/100/70/06', 'Cavities', 170, 0, '1')
	, ('M/100/70/06', 'Colour', 20, 0, 'YELLOW')
	, ('M/100/70/06', 'Colour Number', 10, 0, '-')
	, ('M/100/70/06', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/100/70/06', 'Drawing Issue', 40, 0, '1')
	, ('M/100/70/06', 'Drawing Number', 30, 0, '321554-03')
	, ('M/100/70/06', 'Impressions', 180, 0, '1')
	, ('M/100/70/06', 'Label Type', 70, 0, 'Route Card')
	, ('M/100/70/06', 'Location', 150, 0, 'STORES')
	, ('M/100/70/06', 'Pack Type', 60, 0, 'Assembled')
	, ('M/100/70/06', 'Part Weight', 160, 0, '175g')
	, ('M/100/70/06', 'Quantity/Box', 80, 0, '103')
	, ('M/100/70/06', 'Tool Number', 190, 0, '1437')
	, ('M/101/70/06', 'Cavities', 170, 0, '2')
	, ('M/101/70/06', 'Colour', 20, 0, 'YELLOW')
	, ('M/101/70/06', 'Colour Number', 10, 0, '-')
	, ('M/101/70/06', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/101/70/06', 'Drawing Issue', 40, 0, '1')
	, ('M/101/70/06', 'Drawing Number', 30, 0, '321554-05')
	, ('M/101/70/06', 'Impressions', 180, 0, '2')
	, ('M/101/70/06', 'Label Type', 70, 0, 'Route Card')
	, ('M/101/70/06', 'Location', 150, 0, 'STORES')
	, ('M/101/70/06', 'Pack Type', 60, 0, 'Assembled')
	, ('M/101/70/06', 'Part Weight', 160, 0, '61g')
	, ('M/101/70/06', 'Quantity/Box', 80, 0, '103')
	, ('M/101/70/06', 'Tool Number', 190, 0, '1439')
	, ('M/97/70/06', 'Cavities', 170, 0, '4')
	, ('M/97/70/06', 'Colour', 20, 0, 'YELLOW')
	, ('M/97/70/06', 'Colour Number', 10, 0, '-')
	, ('M/97/70/06', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/97/70/06', 'Drawing Issue', 40, 0, '1')
	, ('M/97/70/06', 'Drawing Number', 30, 0, '321554A')
	, ('M/97/70/06', 'Impressions', 180, 0, '4')
	, ('M/97/70/06', 'Label Type', 70, 0, 'Route Card')
	, ('M/97/70/06', 'Location', 150, 0, 'STORES')
	, ('M/97/70/06', 'Pack Type', 60, 0, 'Assembled')
	, ('M/97/70/06', 'Part Weight', 160, 0, '171g')
	, ('M/97/70/06', 'Quantity/Box', 80, 0, '103')
	, ('M/97/70/06', 'Tool Number', 190, 0, '1440')
	, ('M/99/70/06', 'Cavities', 170, 0, '1')
	, ('M/99/70/06', 'Colour', 20, 0, 'YELLOW')
	, ('M/99/70/06', 'Colour Number', 10, 0, '-')
	, ('M/99/70/06', 'Count Type', 50, 0, 'Weigh Count')
	, ('M/99/70/06', 'Drawing Issue', 40, 0, '1')
	, ('M/99/70/06', 'Drawing Number', 30, 0, '321554A')
	, ('M/99/70/06', 'Impressions', 180, 0, '1')
	, ('M/99/70/06', 'Label Type', 70, 0, 'Route Card')
	, ('M/99/70/06', 'Location', 150, 0, 'STORES')
	, ('M/99/70/06', 'Pack Type', 60, 0, 'Assembled')
	, ('M/99/70/06', 'Part Weight', 160, 0, '171g')
	, ('M/99/70/06', 'Quantity/Box', 80, 0, '103')
	, ('M/99/70/06', 'Tool Number', 190, 0, '1441')
	, ('PC/004', 'Colour', 50, 0, 'YELLOW')
	, ('PC/004', 'Grade', 20, 0, '303EP')
	, ('PC/004', 'Location', 60, 0, 'R2123-9')
	, ('PC/004', 'Material Type', 10, 0, 'PC')
	, ('PC/004', 'Name', 30, 0, 'Calibre')
	, ('PC/004', 'SG', 40, 0, '1.21')
	;
	INSERT INTO Activity.tbOp (ActivityCode, OperationNumber, SyncTypeCode, Operation, Duration, OffsetDays)
	VALUES ('M/00/70/06', 10, 0, 'ASSEMBLE', 0.5, 3)
	, ('M/00/70/06', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/00/70/06', 30, 0, 'PACK', 0, 1)
	, ('M/00/70/06', 40, 2, 'DELIVER', 0, 1)
	, ('M/100/70/06', 10, 0, 'MOULD', 10, 2)
	, ('M/100/70/06', 20, 1, 'INSERTS', 0, 0)
	, ('M/100/70/06', 30, 0, 'QUALITY CHECK', 0, 0)
	, ('M/101/70/06', 10, 0, 'MOULD', 10, 2)
	, ('M/101/70/06', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/97/70/06', 10, 0, 'MOULD', 10, 2)
	, ('M/97/70/06', 20, 0, 'QUALITY CHECK', 0, 0)
	, ('M/99/70/06', 10, 0, 'MOULD', 0, 2)
	, ('M/99/70/06', 20, 0, 'QUALITY CHECK', 0, 0)
	;
	INSERT INTO Activity.tbFlow (ParentCode, StepNumber, ChildCode, SyncTypeCode, OffsetDays, UsedOnQuantity)
	VALUES ('M/00/70/06', 10, 'M/100/70/06', 1, 0, 8)
	, ('M/00/70/06', 20, 'M/101/70/06', 1, 0, 4)
	, ('M/00/70/06', 30, 'M/97/70/06', 1, 0, 3)
	, ('M/00/70/06', 40, 'M/99/70/06', 0, 0, 2)
	, ('M/00/70/06', 50, 'BOX/41', 1, 0, 1)
	, ('M/00/70/06', 60, 'PALLET/01', 1, 0, 0.01)
	, ('M/00/70/06', 70, 'DELIVERY', 2, 1, 0)
	, ('M/100/70/06', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/100/70/06', 20, 'PC/004', 1, 0, 0.175)
	, ('M/101/70/06', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/101/70/06', 20, 'PC/004', 1, 0, 0.061)
	, ('M/97/70/06', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/97/70/06', 20, 'PC/004', 1, 0, 0.172)
	, ('M/99/70/06', 10, 'BOX/99', 1, 0, 0.01)
	, ('M/99/70/06', 20, 'PC/004', 1, 0, 0.171)
	, ('M/100/70/06', 30, 'INSERT/09', 1, 0, 2)
	;


/**********************************************************************************************************/

	SET @AccountCode = 'HOME';
	SELECT @UserId = UserId FROM Usr.vwCredentials;

	DECLARE activities CURSOR LOCAL FOR
		SELECT ActivityCode FROM Activity.tbActivity WHERE CashCode = '103'
		EXCEPT
		SELECT 'M/00/70/00' ActivityCode

	OPEN activities;
	FETCH NEXT FROM activities into @ActivityCode;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXEC Task.proc_NextCode 'PROJECT', @ParentTaskCode OUTPUT
		INSERT INTO Task.tbTask
									(TaskCode, UserId, AccountCode, TaskTitle, ActivityCode, TaskStatusCode, ActionById, ActionOn)
		VALUES        (@ParentTaskCode, @UserId, @AccountCode, N'PIGEON HOLE SHELF ASSEMBLY', N'PROJECT', 0, @UserId, CAST(CURRENT_TIMESTAMP AS DATE))

		SET @ProdCounter = 0;  
		SELECT @MaxOrders = COUNT(*) FROM App.tbYearPeriod WHERE StartOn >= CURRENT_TIMESTAMP;
		SET @Quantity = ROUND(RAND() *10000, 0) 
		SET @MaxOrders = @Quantity % @MaxOrders;
		SET @MaxOrders = CASE @MaxOrders WHEN 0 THEN 2 ELSE @MaxOrders END; 

		WHILE @ProdCounter < @MaxOrders  
		BEGIN  
			SET @Quantity = ROUND(RAND() *10000, 0) 

			SET @ActionOn = CAST(DATEADD(MONTH, @ProdCounter, CURRENT_TIMESTAMP) AS DATE)

			EXEC Task.proc_NextCode @ActivityCode, @TaskCode OUTPUT;
			
			WITH reps AS
			(
				SELECT ROW_NUMBER() OVER (ORDER BY UserId) - 1 RowNo, UserId 
				FROM Usr.tbUser WHERE IsAdministrator = 0
			)
			SELECT @UserId = UserId
			FROM reps
			WHERE RowNo = @Quantity % (SELECT COUNT(*) FROM Usr.tbUser WHERE IsAdministrator = 0)

			INSERT INTO Task.tbTask
					(TaskCode, UserId, AccountCode, TaskTitle, ActivityCode, TaskStatusCode, ActionById, TaskNotes, Quantity, CashCode, TaxCode, UnitCharge, AddressCodeFrom, AddressCodeTo)
			SELECT @TaskCode,@UserId, @AccountCode, N'PIGEON HOLE SHELF ASSEMBLY', ActivityCode, 1,@UserId, 'PIGEON HOLE SHELF ASSEMBLY', @Quantity, cash.CashCode, cash.TaxCode, activity.UnitCharge, @AddressCode, @AddressCode
			FROM Activity.tbActivity activity JOIN Cash.tbCode cash ON activity.CashCode = cash.CashCode
			WHERE ActivityCode = @ActivityCode

			EXEC Task.proc_Configure @TaskCode;
			EXEC Task.proc_AssignToParent @TaskCode, @ParentTaskCode;

			UPDATE Task.tbTask SET ActionOn = @ActionOn WHERE TaskCode = @TaskCode;
			EXEC Task.proc_Schedule @TaskCode;

			SET @ProdCounter += 1  
		END;  
		FETCH NEXT FROM activities into @ActivityCode;
	END

	CLOSE activities;
	DEALLOCATE activities;

	UPDATE Task.tbTask
	SET AccountCode = 'STOBOX'
	WHERE CashCode = '103' AND AccountCode <> 'STOBOX'

	UPDATE Task.tbTask
	SET AccountCode = 'PACSER', ContactName = 'John OGroats', AddressCodeFrom = 'PACSER_001'
	WHERE ActivityCode = 'BOX/41' AND TaskStatusCode = 1;

	UPDATE Task.tbTask
	SET AccountCode = 'TFCSPE', ContactName = 'Gary Granger', AddressCodeFrom = 'TFCSPE_001'
	WHERE ActivityCode = 'INSERT/09' AND TaskStatusCode = 1;

	UPDATE Task.tbTask
	SET AccountCode = 'PALSUP', ContactName = 'Allan Rain', AddressCodeFrom = 'PALSUP_001', CashCode = NULL, UnitCharge = 0 
	WHERE ActivityCode = 'PALLET/01' AND TaskStatusCode = 1;

	UPDATE Task.tbTask
	SET AccountCode = 'PLAPRO', ContactName = 'Kim Burnell', AddressCodeFrom = 'PLAPRO_001'
	WHERE (ActivityCode LIKE N'PC/%') AND TaskStatusCode = 1;
		
	UPDATE Task.tbTask
	SET AccountCode = 'HAULOG', ContactName = 'John Iron',  AddressCodeFrom = 'HOME_001', Quantity = 1, UnitCharge = 250, TotalCharge = 250 --, AddressCodeTo = @AddressCode 
	WHERE ActivityCode = 'DELIVERY' AND TaskStatusCode = 1;

	UPDATE Task.tbTask
	SET AccountCode = (SELECT AccountCode FROM App.tbOptions), ContactName = (SELECT UserName FROM Usr.vwCredentials)
	WHERE (CashCode IS NULL) AND (AccountCode <> 'PALSUP');

END TRY
BEGIN CATCH
	EXEC App.proc_ErrorLog;
END CATCH
GO  

