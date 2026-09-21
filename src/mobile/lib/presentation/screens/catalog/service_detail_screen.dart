import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../core/di/injection_container.dart';
import '../../../data/models/service_dto.dart';
import '../../blocs/booking/booking_bloc.dart';
import '../booking/select_datetime_screen.dart';

class ServiceDetailScreen extends StatelessWidget {
  final ServiceDto service;

  const ServiceDetailScreen({super.key, required this.service});

  @override
  Widget build(BuildContext context) {
    const primaryColor = Color(0xFFC5A059);
    const backgroundColor = Color(0xFF121214);
    const surfaceColor = Color(0xFF1E1E24);
    const textColor = Color(0xFFF5F5F7);
    const subtitleColor = Color(0xFFA0A0AB);

    return Scaffold(
      backgroundColor: backgroundColor,
      body: CustomScrollView(
        slivers: [
          // Collapsible AppBar with Hero Image
          SliverAppBar(
            expandedHeight: 280,
            pinned: true,
            backgroundColor: surfaceColor,
            leading: IconButton(
              icon: Container(
                padding: const EdgeInsets.all(6),
                decoration: const BoxDecoration(
                  color: Color(0x99000000),
                  shape: BoxShape.circle,
                ),
                child: const Icon(Icons.arrow_back_ios_new, color: Colors.white, size: 18),
              ),
              onPressed: () => Navigator.of(context).pop(),
            ),
            flexibleSpace: FlexibleSpaceBar(
              background: Stack(
                fit: StackFit.expand,
                children: [
                  if (service.imagenUrl != null && service.imagenUrl!.isNotEmpty)
                    CachedNetworkImage(
                      imageUrl: service.imagenUrl!,
                      fit: BoxFit.cover,
                      placeholder: (_, __) => Container(color: surfaceColor),
                      errorWidget: (_, __, ___) => Container(
                        color: surfaceColor,
                        child: const Icon(Icons.spa_rounded, color: primaryColor, size: 50),
                      ),
                    )
                  else
                    Container(
                      color: surfaceColor,
                      child: const Icon(Icons.spa_rounded, color: primaryColor, size: 50),
                    ),
                  // Gradient Overlay
                  const DecoratedBox(
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        colors: [
                          Colors.transparent,
                          Color(0xFF121214),
                        ],
                        begin: Alignment.topCenter,
                        end: Alignment.bottomCenter,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),

          // Content
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.all(22.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Category badge & Code
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      if (service.categoriaNombre != null)
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 5),
                          decoration: BoxDecoration(
                            color: const Color(0x2EC5A059),
                            borderRadius: BorderRadius.circular(16),
                            border: Border.all(color: primaryColor, width: 1),
                          ),
                          child: Text(
                            service.categoriaNombre!.toUpperCase(),
                            style: const TextStyle(
                              color: primaryColor,
                              fontSize: 11,
                              fontWeight: FontWeight.bold,
                              letterSpacing: 1.0,
                            ),
                          ),
                        ),
                      Text(
                        service.codigoServicio,
                        style: const TextStyle(color: subtitleColor, fontSize: 12),
                      ),
                    ],
                  ),
                  const SizedBox(height: 12),

                  // Service Title
                  Text(
                    service.nombre,
                    style: const TextStyle(
                      color: textColor,
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                      height: 1.25,
                    ),
                  ),
                  const SizedBox(height: 16),

                  // Price and Duration Info Card
                  Container(
                    padding: const EdgeInsets.all(16),
                    decoration: BoxDecoration(
                      color: surfaceColor,
                      borderRadius: BorderRadius.circular(16),
                      border: Border.all(color: const Color(0x33C5A059)),
                    ),
                    child: Row(
                      mainAxisAlignment: MainAxisAlignment.spaceAround,
                      children: [
                        _infoColumn('PRECIO BASE', service.precioFormateado, primaryColor, textColor, subtitleColor),
                        Container(width: 1, height: 36, color: const Color(0xFF2E2E38)),
                        _infoColumn('DURACIÓN', service.duracionFormateada, primaryColor, textColor, subtitleColor),
                        Container(width: 1, height: 36, color: const Color(0xFF2E2E38)),
                        _infoColumn('SEGUIMIENTO', '${service.intervaloSeguimientoDias} días', primaryColor, textColor, subtitleColor),
                      ],
                    ),
                  ),
                  const SizedBox(height: 24),

                  // Description
                  const Text(
                    'Descripción del Servicio',
                    style: TextStyle(
                      color: textColor,
                      fontSize: 17,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    service.descripcion,
                    style: const TextStyle(
                      color: subtitleColor,
                      fontSize: 15,
                      height: 1.5,
                    ),
                  ),
                  const SizedBox(height: 28),

                  // Quality & Hygiene Guarantee Card
                  Container(
                    padding: const EdgeInsets.all(16),
                    decoration: BoxDecoration(
                      color: const Color(0xFF1E1E24),
                      borderRadius: BorderRadius.circular(14),
                    ),
                    child: const Row(
                      children: [
                        Icon(Icons.verified_rounded, color: primaryColor, size: 24),
                        SizedBox(width: 12),
                        Expanded(
                          child: Text(
                            'Material esterilizado y productos de grado profesional garantizados.',
                            style: TextStyle(color: textColor, fontSize: 13),
                          ),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 100), // spacing for bottom bar
                ],
              ),
            ),
          ),
        ],
      ),
      bottomSheet: Container(
        padding: const EdgeInsets.symmetric(horizontal: 22, vertical: 16),
        color: surfaceColor,
        child: SafeArea(
          child: Row(
            children: [
              Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text('Total Estimado', style: TextStyle(color: subtitleColor, fontSize: 12)),
                  Text(
                    service.precioFormateado,
                    style: const TextStyle(
                      color: primaryColor,
                      fontSize: 20,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ],
              ),
              const SizedBox(width: 20),
              Expanded(
                child: ElevatedButton(
                  onPressed: () {
                    Navigator.of(context).push(
                      MaterialPageRoute(
                        builder: (_) => BlocProvider<BookingBloc>(
                          create: (_) => sl<BookingBloc>(),
                          child: SelectDateTimeScreen(service: service),
                        ),
                      ),
                    );
                  },
                  style: ElevatedButton.styleFrom(
                    backgroundColor: primaryColor,
                    foregroundColor: Colors.black,
                    padding: const EdgeInsets.symmetric(vertical: 16),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(14),
                    ),
                  ),
                  child: const Text(
                    'Reservar Cita',
                    style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _infoColumn(
    String label,
    String value,
    Color primaryColor,
    Color textColor,
    Color subtitleColor,
  ) {
    return Column(
      children: [
        Text(label, style: TextStyle(color: subtitleColor, fontSize: 10, letterSpacing: 0.8)),
        const SizedBox(height: 4),
        Text(value, style: TextStyle(color: textColor, fontSize: 15, fontWeight: FontWeight.bold)),
      ],
    );
  }
}
