using AutoMapper;
using FastX_BusTicketBooking.API.Contexts;
using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;
using FastX_BusTicketBooking.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using log4net;

namespace FastX_BusTicketBooking.API.Services.Implementations
{
    public class BusService : IBusService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        public BusService(AppDbContext context, IMapper mapper, ILog logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BusDTO>> GetAllBuses()
        {
            try
            {
                _logger.Info("Fetching all buses...");
                var buses = await _context.Buses
                    .Where(b => !b.IsDeleted)
                    .Include(b => b.Routes)
                    .ToListAsync();

                _logger.Info($"Total buses fetched: {buses.Count}");
                return _mapper.Map<IEnumerable<BusDTO>>(buses);
            }
            catch (Exception ex)
            {
                _logger.Error("Error fetching all buses", ex);
                throw new Exception($"Error fetching buses: {ex.Message}");
            }
        }

        public async Task<BusDTO?> GetBusById(int id)
        {
            try
            {
                _logger.Info($"Fetching bus by ID: {id}");
                var bus = await _context.Buses
                    .Include(b => b.Routes)
                    .FirstOrDefaultAsync(b => b.BusId == id && !b.IsDeleted);

                if (bus == null)
                {
                    _logger.Warn($"Bus not found with ID: {id}");
                    return null;
                }

                _logger.Info($"Bus found: BusId={id}");
                return _mapper.Map<BusDTO>(bus);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching bus by ID={id}", ex);
                throw new Exception($"Error fetching bus: {ex.Message}");
            }
        }

        public async Task<string> AddBus(BusDTO busDTO)
        {
            try
            {
                _logger.Info($"Attempting to add bus: BusNumber={busDTO.BusNumber}");

                if (await _context.Buses.AnyAsync(b => b.BusNumber == busDTO.BusNumber))
                {
                    _logger.Warn($"Bus already exists with BusNumber={busDTO.BusNumber}");
                    return $"Bus with number {busDTO.BusNumber} already exists.";
                }

                var bus = _mapper.Map<Bus>(busDTO);
                _context.Buses.Add(bus);
                await _context.SaveChangesAsync();

                _logger.Info($"Bus added successfully: BusId={bus.BusId}");
                return "Bus added successfully.";
            }
            catch (Exception ex)
            {
                _logger.Error("Error adding bus", ex);
                return $"Error adding bus: {ex.Message}";
            }
        }

        public async Task<string> UpdateBus(int id, BusDTO busDTO)
        {
            try
            {
                _logger.Info($"Attempting to update bus: BusId={id}");

                var bus = await _context.Buses.FindAsync(id);
                if (bus == null)
                {
                    _logger.Warn($"Bus not found to update: BusId={id}");
                    return "Bus not found to update.";
                }

                _mapper.Map(busDTO, bus);
                await _context.SaveChangesAsync();

                _logger.Info($"Bus updated successfully: BusId={id}");
                return "Bus updated successfully.";
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating bus: BusId={id}", ex);
                return $"Error updating bus: {ex.Message}";
            }
        }

        public async Task<string> DeleteBus(int id)
        {
            try
            {
                _logger.Info($"Attempting to delete bus: BusId={id}");

                var bus = await _context.Buses.FirstOrDefaultAsync(b => b.BusId == id && !b.IsDeleted);
                if (bus == null)
                {
                    _logger.Warn($"Bus not found to delete: BusId={id}");
                    return "Bus not found to delete.";
                }

                bus.IsDeleted = true;
                await _context.SaveChangesAsync();

                _logger.Info($"Bus deleted successfully: BusId={id}");
                return "Bus soft-deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting bus: BusId={id}", ex);
                return $"Error deleting bus: {ex.Message}";
            }
        }
    }
}
