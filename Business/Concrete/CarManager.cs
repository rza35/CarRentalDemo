﻿using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Validation;
using Business.BusinessAspects.Autofac;
using Core.Aspects.Autofac.Caching;
using System.Linq;
using Core.Utilities.Business;

namespace Business.Concrete
{
    public class CarManager : ICarService
    {
        ICarDal _carDal;

        public CarManager(ICarDal carDal)
        {
            _carDal = carDal;
        }
        [SecuredOperation("car.add,admin")]
        [ValidationAspect(typeof(CarValidator))]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Add(Car car)
        {
            IResult result = BusinessRules.Run(CheckIfCarNameExists(car.CarName));
            _carDal.Add(car);

            return new Result(true, Messages.CarAdded);
        }

        public IResult Delete(Car car)
        {
            _carDal.Delete(car);

            return new Result(true, Messages.CarDeleted);
        }


        [CacheAspect]
        public IDataResult<List<Car>> GetAll()
        {


            return new SuccessDataResult<List<Car>>(_carDal.GetAll(), Messages.CarListed);
        }

        public IDataResult<List<CarDetailDto>> GetCarDetails()
        {
            return new SuccessDataResult<List<CarDetailDto>>(_carDal.GetCarDetails());
        }

        [ValidationAspect(typeof(CarValidator))]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Update(Car car)
        {
            
            _carDal.Update(car);

            return new Result(true, Messages.CarUpdated);
        }

        public IDataResult<Car> GetCarByColorId(int colorId)
        {
            return new SuccessDataResult<Car>(_carDal.Get(c => c.ColorId == colorId), Messages.CarListed);
        }

        public IDataResult<Car> GetCarByBrandId(int brandId)
        {
            return new SuccessDataResult<Car>(_carDal.Get(c => c.BrandId == brandId), Messages.CarListed);
        }

        public IDataResult<List<Car>> GetAllByCategoryId(int id)
        {
            throw new NotImplementedException();
        }

        public IDataResult<List<Car>> GetByUnitPrice(decimal min, decimal max)
        {
            throw new NotImplementedException();
        }


        [CacheAspect]
        public IDataResult<List<Car>> GetById(int carId)
        {
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(c => c.Id == carId), Messages.CarListed);
        }

        private IResult CheckIfCarNameExists(string productName)
        {
            var result = _carDal.GetAll(p => p.CarName == productName).Any();
            if (result)
            {
                return new ErrorResult(Messages.CarNameAlreadyExists);
            }

            return new ErrorResult(Messages.CarNameAlreadyExists);
        }
    }
}
