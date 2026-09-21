import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class TokenStorage {
  static const _keyJwtToken = 'jwt_access_token';
  static const _keyUserId = 'current_user_id';
  static const _keyUserLogin = 'current_user_login';
  static const _keyUserRole = 'current_user_role';
  static const _keyUserName = 'current_user_name';

  final FlutterSecureStorage _storage;

  TokenStorage({FlutterSecureStorage? storage})
      : _storage = storage ??
            const FlutterSecureStorage(
              aOptions: AndroidOptions(encryptedSharedPreferences: true),
              iOptions: IOSOptions(
                accessibility: KeychainAccessibility.first_unlock,
              ),
            );

  Future<void> saveToken(String token) async {
    await _storage.write(key: _keyJwtToken, value: token);
  }

  Future<String?> getToken() async {
    return await _storage.read(key: _keyJwtToken);
  }

  Future<void> deleteToken() async {
    await _storage.delete(key: _keyJwtToken);
  }

  Future<void> saveUserData({
    required int id,
    required String login,
    required String role,
    String? name,
  }) async {
    await _storage.write(key: _keyUserId, value: id.toString());
    await _storage.write(key: _keyUserLogin, value: login);
    await _storage.write(key: _keyUserRole, value: role);
    if (name != null) {
      await _storage.write(key: _keyUserName, value: name);
    }
  }

  Future<Map<String, String?>> getUserData() async {
    final id = await _storage.read(key: _keyUserId);
    final login = await _storage.read(key: _keyUserLogin);
    final role = await _storage.read(key: _keyUserRole);
    final name = await _storage.read(key: _keyUserName);
    return {
      'id': id,
      'login': login,
      'role': role,
      'name': name,
    };
  }

  Future<void> clearAll() async {
    await _storage.deleteAll();
  }
}
