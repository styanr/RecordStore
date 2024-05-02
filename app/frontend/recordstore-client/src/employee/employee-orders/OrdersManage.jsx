import OrderSection from '../../profile/OrderSection';

import useOrders from '../../hooks/useOrders';

import { useEffect, useState } from 'react';

import useAuth from '../../hooks/useAuth';

import {
  Box,
  Container,
  Heading,
  useToast,
  Button,
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalFooter,
  ModalBody,
  ModalCloseButton,
  useDisclosure,
  Select,
  Input,
  Flex,
} from '@chakra-ui/react';

const OrdersManage = () => {
  const toast = useToast();
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [exportParams, setExportParams] = useState({
    format: 'json',
    from: '',
    to: '',
  });

  const { user } = useAuth();

  const {
    orders,
    statusOptions,
    fetchOrders,
    updateOrderStatus,
    exportOrders,
  } = useOrders('employee');

  const handleUpdateStatus = async (orderId, status) => {
    const response = await updateOrderStatus(orderId, status);
    console.log(response);
    if (response.success === true) {
      toast({
        title: 'Статус замовлення оновлено',
        status: 'success',
        duration: 5000,
        isClosable: true,
      });
    }
    if (response.success === false) {
      toast({
        title: 'Помилка оновлення статусу замовлення',
        description: response.error,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }

    return response;
  };

  const handleExport = async () => {
    const response = await exportOrders(exportParams);
    if (response.success === true) {
      toast({
        title: 'Дані експортовано',
        status: 'success',
        duration: 5000,
        isClosable: true,
      });
    }
    if (response.success === false) {
      toast({
        title: 'Помилка експорту даних',
        description: response.error,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }
    onClose();
  };

  return (
    <Box bg='gray.100' py={12} flexGrow={1}>
      <Container maxW='7xl'>
        <Flex justify='space-between' align='center' mb={4}>
          <Heading>Керування замовленнями</Heading>
          {user && user.role === 'admin' && (
            <Button onClick={onOpen} mb={4} colorScheme='pink'>
              Експортувати
            </Button>
          )}
        </Flex>
        <Modal isOpen={isOpen} onClose={onClose}>
          <ModalOverlay />
          <ModalContent>
            <ModalHeader>Експорт даних</ModalHeader>
            <ModalCloseButton />
            <ModalBody>
              <Select
                value={exportParams.format}
                onChange={(e) =>
                  setExportParams({ ...exportParams, format: e.target.value })
                }
                mb={4}
              >
                <option value='json'>JSON</option>
                <option value='xml'>XML</option>
                <option value='csv'>CSV</option>
              </Select>
              <Input
                type='date'
                value={exportParams.from}
                onChange={(e) =>
                  setExportParams({ ...exportParams, from: e.target.value })
                }
                mb={4}
              />
              <Input
                type='date'
                value={exportParams.to}
                onChange={(e) =>
                  setExportParams({ ...exportParams, to: e.target.value })
                }
                mb={4}
              />
            </ModalBody>
            <ModalFooter>
              <Button colorScheme='blue' mr={3} onClick={handleExport}>
                Експортувати
              </Button>
              <Button variant='ghost' onClick={onClose}>
                Скасувати
              </Button>
            </ModalFooter>
          </ModalContent>
        </Modal>
        <OrderSection
          orders={orders}
          fetchOrders={fetchOrders}
          manage
          statusOptions={statusOptions}
          updateStatus={handleUpdateStatus}
        />
      </Container>
    </Box>
  );
};

export default OrdersManage;
