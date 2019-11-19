
CREATE proc [dbo].[_IsCastValid]

	@PropValue nvarchar(max),
	@DataTypeID smallint,
	@DataTypeLength int,
	@DataTypePrecision nchar(10)

as
begin

	declare @DataType nvarchar(100)
	
	select
		@DataType = [Name]
	from dbo.DataTypes
	where
		ID = @DataTypeID


	--validate datatype to prevent injection attacks
	if(@DataType is not NULL)
	begin

		--NULL value will pass conversion to any data type
		--No need to run complicated eval code
		if(@PropValue is NULL)
		begin
			return 1;
		end


		begin try
			--try to cast @Value into variable
			--if the cast is successful, return True
			--if the cast throws any errors, return False
			--perform extra end check to validate output against input - forces LENGTH validation for types like nvarchar(N)
			----this check will not prevent decimal rounding
			----if rounding validation is needed, cast @output to nvarchar
			declare @sqlCmd nvarchar(max)
			
			--0 -> (length)
			--1 -> (precision)
			--2 -> (const)
			declare @DataTypeZone tinyint = (@DataTypeID / 100)
			declare @PrecisionSuffix nchar(20) = ''

			--length based types
			if(@DataTypeZone = 0)
			begin
				--no type validation needed because the only supported length based type is nvarchar
				--only length validation is req

				--ASIDE
				--Append x to string values to acount for end-of-line whitespace truncation
				--This allows for proper string length comparisons
				--Without this, then '  ' == ''
				--
				--set @output = @output + ''x''
				--set @val = @val + ''x''


				set @PropValue = @PropValue + 'x'

				--MAX Length
				if(@DataTypeLength = -1)
				begin
					return cast(1 as bit)
				end

				if(LEN(@PropValue) <= @DataTypeLength)
				begin 
					return  cast(1 as bit)
				end
				else begin
					return cast(0 as bit)
				end

			end
			--precision based types
			else if(@DataTypeZone = 1)
			begin

				set @PrecisionSuffix = '(' + @DataTypePrecision + ')'

			end
			--const types
			else if(@DataTypeZone = 2)
			begin

				set @PrecisionSuffix = ''

			end
			

			declare @ActualDataType nvarchar(150) = @DataType + @PrecisionSuffix

			set @sqlCmd = 
			N'declare @output '+ @ActualDataType + '
			set @output = TRY_CONVERT(' + @ActualDataType + ', @val)
			if(@output is null)
			begin
				set @retValOUT = cast(0 as bit)
			end
			else begin
				set @retValOUT = cast(1 as bit)
			end'

			DECLARE @retVal bit
			EXEC sp_executesql
				@sqlCmd,
				N'@val nvarchar(max), @retValOUT bit OUTPUT',
				@retValOUT = @retVal OUTPUT,
				@val = @PropValue

			return @retVal


		end try
		begin catch
			return cast(0 as bit)
		end catch

	end
	else begin
		return cast(0 as bit)
	end

end