using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABATON.Controllers;
using Microsoft.EntityFrameworkCore;
using ABATON.Models;
using ABATON.Services;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;


namespace ABATON.Test
{
    public class PatientsControllerTest : IDisposable
    {
        private readonly PatientContext _context;
        private readonly Mock<IRelationshipService> _relationshipMock;

        public PatientsControllerTest()
        {
            var options = new DbContextOptionsBuilder<PatientContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _context = new PatientContext(options);

            var patients = new[]
            {
                new Patient{Id = 1, Name = "John Smith" },
                new Patient{Id = 2, Name = "Alan White" },
                new Patient{Id = 3, Name = "Jack Black" },
                new Patient{Id = 4, Name = "Sean Patrick" }
            };

            _context.Patients.AddRange(patients);

            _context.SaveChanges();

            _relationshipMock = new Mock<IRelationshipService>();            
        }

        [Fact]
        public async Task GetPatients_ReturnsCorrectType()
        {
            var controller = new PatientsController(_context, _relationshipMock.Object);

            var result = await controller.GetPatients();          

            Assert.IsAssignableFrom<IEnumerable<Patient>>(result.Value);
        }

        [Fact]
        public async Task GetPatients_ReturnsAllPatients()
        {
            var controller = new PatientsController(_context, _relationshipMock.Object);

            var result = await controller.GetPatients();

            var patients = Assert.IsAssignableFrom<IEnumerable<Patient>>(result.Value);

            Assert.Equal(4, patients.Count());
        }

        [Fact]
        public async Task GetPatient_ReturnsCorrectType()
        {
            var controller = new PatientsController(_context, _relationshipMock.Object);

            var result = await controller.GetPatient(1);

            Assert.IsType<Patient>(result.Value);
        }

        [Fact]
        public async Task GetPatient_ReturnsCorrectPatient()
        {
            var controller = new PatientsController(_context, _relationshipMock.Object);

            var result = await controller.GetPatient(2);

            var patient = Assert.IsType<Patient>(result.Value);

            Assert.Equal("Alan White", patient.Name);
            Assert.False(patient.Deleted);
        }

        [Fact]
        public async Task PutPatient_UpdatesPatient()
        {
            var controller = new PatientsController(_context, _relationshipMock.Object);

            long id = 4;

            Patient patient = new Patient();

            patient.Id = id;
            patient.Name = "Put Patient";

            await controller.PutPatient(id,patient);

            var result = await controller.GetPatient(id);

            var patientResult = Assert.IsType<Patient>(result.Value);

            Assert.Equal(id, patient.Id);
            Assert.False(patient.Deleted);
            Assert.Equal("Put Patient", patientResult.Name);
        }

        [Fact]
        public async Task PutRestoredPatient_ReturnsBadRequest()
        {
            var controller = new PatientsController(_context, _relationshipMock.Object);

            long id = 4;

            await controller.DeletePatient(id);

            Patient patient = new Patient();

            patient.Id = id;
            patient.Name = "Put Patient";
            patient.Deleted = false;           

            var result = await controller.PutPatient(id, patient);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task PostPatient_AddsPatient()
        {
            var controller = new PatientsController(_context, _relationshipMock.Object);

            long id = 5;

            Patient patient = new Patient();

            patient.Id = id;
            patient.Name = "Post Patient";

            await controller.PostPatient(patient);

            var result = await controller.GetPatient(id);

            var patientResult = Assert.IsType<Patient>(result.Value);

            Assert.Equal(id, patient.Id);
            Assert.False(patient.Deleted);
            Assert.Equal("Post Patient", patientResult.Name);
        }

        [Fact]
        public async Task PostPatientDeleted_ReturnsBadRequest()
        {
            var controller = new PatientsController(_context, _relationshipMock.Object);

            long id = 5;

            Patient patient = new Patient();

            patient.Id = id;
            patient.Name = "Post Patient";
            patient.Deleted = true;
                     
            var result = await controller.PostPatient(patient);

            Assert.IsAssignableFrom<BadRequestObjectResult> (result.Result);
                        
        }

        [Fact]
        public async Task DeletePatient_SetDeletedFlag()
        {
            var controller = new PatientsController(_context, _relationshipMock.Object);

            long id = 1;
                        
            await controller.DeletePatient(id);

            _relationshipMock.Verify(mock => mock.DeleteDosagePatientId(id), Times.Once);

            var result = await controller.GetPatient(id);

            var patientResult = Assert.IsType<Patient>(result.Value);

            Assert.Equal(id, patientResult.Id);
            Assert.True(patientResult.Deleted);
            Assert.Equal("John Smith", patientResult.Name);
        }

        [Fact]
        public async Task DeleteAlreadyDeletedPatient_ReturnsBadRequest()
        {
            var controller = new PatientsController(_context, _relationshipMock.Object);

            long id = 1;

            await controller.DeletePatient(id);

            _relationshipMock.Verify(mock => mock.DeleteDosagePatientId(id), Times.Once);

            var result = await controller.DeletePatient(id);

            //verify that DeleteDosagePatientId did not get called again 
            //for an already deleted patient
            _relationshipMock.Verify(mock => mock.DeleteDosagePatientId(id), Times.Once);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);

        }
        
        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
