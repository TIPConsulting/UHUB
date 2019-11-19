
CREATE procedure [dbo].[_vEnts_Helper]

	@EntID int,
	@EntTypeID int,
	@PropID int,
	@PropValue nvarchar(max),
	@ModifiedBy bigint,
	@ModifiedDate datetimeoffset(6),
	@IsNewRecord bit

as
begin

	--empty values are pointless in this scheme
	--it takes less space to store an XREF null
	if(@PropValue = '')
	begin
		set @PropValue = null
	end

	--empty values are pointless in this scheme
	--it takes less space to store an XREF null

	begin try

		declare @_errorMsg nvarchar(500)

		declare @PropFriendlyName nvarchar(100)
		declare @DataTypeID smallint
		declare @DataType varchar(100)
		declare @IsNullable bit
		declare @DefaultValue nvarchar(MAX)
		declare @ActualLength int
		declare @ActualPrecision nchar(10)
		declare @DataTypeSuffix nchar(10)

		--get info about current modified data property
		--CONSTRAINED to the current entity type
		select 
			@PropFriendlyName = ep.PropFriendlyName,
			@DataTypeID = epm.DataTypeID,
			@DataType = ep.DataType,
			@IsNullable = epm.IsNullable,
			@DefaultValue = epm.DefaultValue,
			@ActualLength = COALESCE(epm.OverrideLength, ep.DefaultLength),
			@ActualPrecision = COALESCE(epm.OverridePrecision, ep.DefaultPrecision)
		from dbo.EntPropertyMap epm
		left join dbo.EntProperties ep
		on
			ep.ID = @PropID
		where
			epm.EntTypeID = @EntTypeID
			and epm.PropID = @PropID


		--ensure that the specified entity can have the specified property
		--#EntProp_Current will have no records if the prop is not valid
		if(@PropFriendlyName is null)
		begin
			--get prop friendly ID from table
			select @_errorMsg = '400: ''' + [PropFriendlyName] + ''' is invalid for the specified entity type'
			from dbo.EntProperties
			where
				ID = @PropID


			;throw 51000, @_errorMsg, 1;
		end


			
		declare @_canWriteToHistory bit = 0
		--try to cast the value to proper data type
		--if it fails, throw error
		declare @_out bit
		exec @_out = dbo._IsCastValid
						@PropValue = @PropValue, 
						@DataTypeID = @DataTypeID, 
						@DataTypeLength = @ActualLength, 
						@DataTypePrecision = @ActualPrecision

		if(@_out = 0)
		begin
			set @_errorMsg = '400: Supplied value for ''' + @PropFriendlyName + ''' cannot be converted to proper data type [''' + @DataType + ''']'

			;throw 51000, @_errorMsg, 1;
		end
		

		--nonNullable types need extra validation
		if(@IsNullable = 0)
		begin
			--(new) throw error if user doesnt supply a value and there is no default
			--(existing) throw error if user doesnt supply a value
			if(@PropValue is NULL AND (@DefaultValue  is NULL OR @IsNewRecord = 0))
			begin
				set @_errorMsg = '400: Entity property ''' + @PropFriendlyName + ''' cannot be null or empty'

				;throw 51000, @_errorMsg, 1;
			end
			--(new) use default value if user doesnt supply one
			else if(@IsNewRecord = 1 AND @PropValue is NULL AND @DefaultValue  is not NULL)
			begin
				set @PropValue = @DefaultValue 
			end
		end


		--proceed after data validation

		--update/delete existing records
		declare @CurrentPropVal nvarchar(MAX)
		declare @CurrentPropCreatedDate datetimeoffset(7)

		select 
			@CurrentPropVal = PropValue,
			@CurrentPropCreatedDate = CreatedDate
		from dbo.EntPropertyXRef
		where 
			EntTypeID = @EntTypeID 
			AND EntID = @EntID 
			AND PropID = @PropID


		if(@CurrentPropVal is not null)
		begin
			--keep empty prop record stubs
			--if(@PropValue is not NULL)
			--begin
				
				--compare old val to new val to check if the property has actually changed
				--useful for accurate change tracking log
				--do not attempt to simplify null checks
				if(
					(@CurrentPropVal is NULL AND @PropValue is not NULL) OR
					(@CurrentPropVal is not NULL AND @PropValue is NULL) OR
					(@CurrentPropVal != @PropValue)
				)
				begin

					set @_canWriteToHistory = 1
				--update property value
					update dbo.EntPropertyXRef
					set 
						PropValue = @PropValue,
						ModifiedBy = @ModifiedBy,
						ModifiedDate = @ModifiedDate
					where
						EntID = @EntID
						and EntTypeID = @EntTypeID
						AND PropID = @PropID
				end
			

			--end
			--else begin
			----delete standard property
			--	delete dbo.EntPropertyXRef
			--	where
			--		EntTypeID = @EntTypeID
			--		AND EntID = @EntID
			--		AND PropID = @PropID
			--end
		end
		--create new records
		else begin
			--create empty prop record stubs
			--if(@PropValue is not NULL)
			--begin

				
				set @_canWriteToHistory = 1
				--insert standard property
				begin try

					insert into dbo.EntPropertyXRef
						(EntID, EntTypeID, PropID, PropValue, CreatedBy, CreatedDate, ModifiedBy)
					values
						(@EntID, @EntTypeID, @PropID, @PropValue, @ModifiedBy, @ModifiedDate, @ModifiedBy)
					end try

				begin catch
					--ensure property exists
					;throw 51000, '400: Specified property ID does not exist', 1;
				end catch
			
			--end
		end

		--if availble, write to revision history table
		if (@_canWriteToHistory = 1)
		begin
			--force history write for all attributes on record instantiation,
			--otherwise, respect mapping rules
			if(@IsNewRecord = 1 or exists (select EntTypeID from dbo.EntPropertyRevisionMap where EntTypeID = @EntTypeID AND PropID = @PropID))
			begin

				--deprecate last value before adding new value
				--Use slightly lower value to prevent date overlap between old/cuurrent value
				update dbo.EntPropertyRevisionXRef
				set
					DeprecatedDate = DATEADD(MICROSECOND, -1, @ModifiedDate)
				where 
					EntID = @EntID
					and EntTypeID = @EntTypeID
					AND PropID = @PropID


				--add new value
				insert into dbo.EntPropertyRevisionXRef
					(EntID, EntTypeID, PropID, PropValue, CreatedBy, CreatedDate)
				values
					(@EntID, @EntTypeID, @PropID, @PropValue, @ModifiedBy, @ModifiedDate)

			end
			--return update status to caller so the entity 'ModifiedDate' can be updated properly
			--1 indicates new update
			return 1
		end
		else begin
			--return update status to caller so the entity 'ModifiedDate' can be updated properly
			--0 indicates no change
			return 0
		end

		
	end try
	begin catch

		;throw;

	end catch
end
